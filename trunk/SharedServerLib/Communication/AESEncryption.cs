/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 2:36 AM
 * 
 * Orignal source from http://stackoverflow.com/questions/165808/simple-2-way-encryption-for-c/212707#212707
 * by Mark Brittingham
 * 
 * Modifyed by Matthew
 */
 
 /*
  * Notice: 
  * TODO: Possible Exceptions not documented.
  */
 
using System;
using System.Data;
using System.Security.Cryptography;
using System.IO;

namespace SharedServerLib.Communication
{
	/// <summary>
	/// Description of AESEncryption.
	/// </summary>
	public class AESEncryption
	{
		
		private byte[] _key;
		private byte[] _vector;

        private static RijndaelManaged _rm = new RijndaelManaged();

		private ICryptoTransform _encryptorTransform, _decryptorTransform;
		private System.Text.UTF8Encoding UTFEncoder;
		
		public AESEncryption(byte[] key, byte[] vector)
		{
            _key = key;
            _vector = vector;

            _encryptorTransform = _rm.CreateEncryptor(this._key, this._vector);
            _decryptorTransform = _rm.CreateDecryptor(this._key, this._vector);
		    
		    UTFEncoder = new System.Text.UTF8Encoding();
		}
		
		static public byte[] GenerateEncryptionKey()
		{
		    RijndaelManaged rm = new RijndaelManaged();
		    rm.GenerateKey();
		    return rm.Key;
		}
		
		static public byte[] GenerateEncryptionVector()
		{
		    //Generate a Vector
		    RijndaelManaged rm = new RijndaelManaged();
		    rm.GenerateIV();
		    return rm.IV;
		}
		
		

		
		public byte[] Encrypt(byte[] bytes)
		{   
		    //Used to stream the data in and out of the CryptoStream.
		    MemoryStream memoryStream = new MemoryStream();
		
		    /*
		     * We will have to write the unencrypted bytes to the stream,
		     * then read the encrypted result back from the stream.
		     */
		    #region Write the decrypted value to the encryption stream
            CryptoStream cs = new CryptoStream(memoryStream, _encryptorTransform, CryptoStreamMode.Write);
		    cs.Write(bytes, 0, bytes.Length);
		    cs.FlushFinalBlock();
		    #endregion
		
		    #region Read encrypted value back out of the stream
		    memoryStream.Position = 0;
		    byte[] encrypted = new byte[memoryStream.Length];
		    memoryStream.Read(encrypted, 0, encrypted.Length);
		    #endregion
		
		    //Clean up.
		    cs.Close();
		    memoryStream.Close();
		
		    return encrypted;
		}

		public string EncryptToString(string TextValue)
		{
			return ByteArrToString(Encrypt(UTFEncoder.GetBytes(TextValue)));
		}
		
		public string EncryptToString(byte[] Data)
		{
			return ByteArrToString(Encrypt(Data));
		}

		/// The other side: Decryption methods
		public string DecryptString(string EncryptedString)
		{
			return ByteArrToString(Decrypt(StrToByteArray(EncryptedString)));
			
		}
		
		/// Decryption when working with byte arrays.    
		public string DecryptToString(byte[] EncryptedValue)
		{
		    return UTFEncoder.GetString(Decrypt(EncryptedValue));
		}
		
		public byte[] Decrypt(byte[] EncryptedValue)
		{
		    #region Write the encrypted value to the decryption stream
		    MemoryStream encryptedStream = new MemoryStream();
		    CryptoStream decryptStream = new CryptoStream(encryptedStream, _decryptorTransform, CryptoStreamMode.Write);
		    decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
		    decryptStream.FlushFinalBlock();
		    #endregion
		
		    #region Read the decrypted value from the stream.
		    encryptedStream.Position = 0;
		    Byte[] decryptedBytes = new Byte[encryptedStream.Length];
		    encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
		    encryptedStream.Close();
		    #endregion
		    return decryptedBytes;
		}
		
		/// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
		//      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		//      return encoding.GetBytes(str);
		// However, this results in character values that cannot be passed in a URL.  So, instead, I just
		// lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
		public byte[] StrToByteArray(string str)
		{
		    if (str.Length == 0)
		        throw new Exception("Invalid string value in StrToByteArray");
		
		    byte val;
		    byte[] byteArr = new byte[str.Length / 3];
		    int i = 0;
		    int j = 0;
		    do
		    {
		        val = byte.Parse(str.Substring(i, 3));
		        byteArr[j++] = val;
		        i += 3;
		    }
		    while (i < str.Length);
		    return byteArr;
		}
		
		// Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
		//      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
		//      return enc.GetString(byteArr);    
		public string ByteArrToString(byte[] byteArr)
		{
		    byte val;
		    string tempStr = "";
		    for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
		    {
		        val = byteArr[i];
		        if (val < (byte)10)
		            tempStr += "00" + val.ToString();
		        else if (val < (byte)100)
		            tempStr += "0" + val.ToString();
		        else
		            tempStr += val.ToString();
		    }
		    return tempStr;
		}
	}
}
