/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 2:24 AM
 * 
 * Copyright 2010 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using System.Security;

using SharedServerLib.Communication;
using SharedServerLib.Exceptions;
using SharedServerLib.Misc;

namespace LoginServer.XML
{
    /// <summary>
    /// Configeration Database for Login Server
    /// </summary>
    [Serializable]
    public class LoginServerConfig
    {

        private static LoginServerConfig _instance;
        public static LoginServerConfig Instance
        {
            get { return _instance; }
        }
        private const string DefaultConfigPath = "LoginServer.XML.DefaultConfig.xml";


        /// <summary>
        /// This is the name that the server uses when sharing info about its self.
        /// </summary>
        public string ServerName;


        /// <summary>
        /// This is the port the server listen on for clients.
        /// </summary>
        public int ClientListenPort;

        /// <summary>
        /// this is the address the server listens on for clients.
        /// </summary>
        public string ClientListenAddress;

        [System.Xml.Serialization.XmlIgnore]
        public IPAddress ConvertedClientListenAddress;



        /// <summary>
        /// This is the port the server listen on for servers.
        /// </summary>
        public int ServerListenPort;

        /// <summary>
        /// this is the address the server listens on for clients.
        /// </summary>
        public string ServerListenAddress;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public IPAddress ConvertedServerListenAddress;


        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool AcceptAnyAddress;


        public string[] ServerListenAcceptedAddresses;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public IPAddress[] ConvertedAcceptedServerAddresses;



        /// <summary>
        /// This is the Port for the Mysql Database.
        /// </summary>
        public int MysqlPort;

        /// <summary>
        /// This is the address for the Mysql Database.
        /// </summary>
        public string MysqlAddress;

        /// <summary>
        /// This is the Account Databse for the Mysql Database.
        /// </summary>
        public string MysqlAccountDatabse;

        /// <summary>
        /// This is the Server Databse for the Mysql Database.
        /// </summary>	
        public string MysqlServerDatabse;

        /// <summary>
        /// This is the User for the Mysql Database.
        /// </summary>
        public string MysqlUser;

        /// <summary>
        /// This is the Password for the Mysql Database.
        /// </summary>
        public string MysqlPass;

        /// <summary>
        /// This is the number of threads that the server will use to handle Clients.
        /// </summary>
        public int ClientListenThreads;

        /// <summary>
        /// This is the number of Clients each thread will handle.
        /// </summary>
        public int MaxUsersPerThread;




        public string SyncKey
        {
            get
            {
                return _SyncKey;
            }
            set
            {
                _Key = SharedServerLib.Misc.ByteStringConverter.StringToBytes(value);
                _SyncKey = value;
            }
        }
        public byte[] Key
        {
            get
            {
                return _Key;
            }
        }





        private string _SyncKey;
        private byte[] _Key;


        /// <summary>
        /// This creates a default Config and saves it.
        /// </summary>
        /// <exception cref="SharedServerLib.Exceptions.TortusMissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="SharedServerLib.Exceptions.TortusFileException">The configeration cannot be saved.</exception>
        public static void CreateDefault()
        {

            Assembly Selfassembly;
            StreamReader SR;
            String DefaultFile;

            Selfassembly = Assembly.GetExecutingAssembly();


            try
            {
                SR = new StreamReader(Selfassembly.GetManifestResourceStream(DefaultConfigPath));
            }
            catch (ArgumentNullException)
            {
                throw new TortusMissingResourceException("The resource could not be loaded.", DefaultConfigPath);
            }

            DefaultFile = SR.ReadToEnd();

            byte[] Key;
            string cKey;

            Key = AESEncryption.GenerateEncryptionKey();

            cKey = ByteStringConverter.BytesToString(Key);

            DefaultFile = DefaultFile.Replace("{KEY}", cKey);

            try
            {
                File.WriteAllText("./LoginConfig.xml", DefaultFile);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new TortusFileException(string.Format("The application does not have permission to save the file ./LoginConfig.xml"), "./LoginConfig.xml", ex);
            }
            catch (SecurityException ex)
            {
                throw new TortusFileException(string.Format("The application does not have permission to save the file ./LoginConfig.xml"), "./LoginConfig.xml", ex);
            }
        }

        /// <summary>
        /// This Loads the configeration file into the static feild LoginServer.XML.LoginServer.Instance.
        /// </summary>
        /// <exception cref="SharedServerLib.Exceptions.TortusMissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="SharedServerLib.Exceptions.TortusFileException">The configeration cannot be loaded or initally created.</exception>
        /// <exception cref="SharedServerLib.Exceptions.TortusFormatException">An IP or Hostname is invalid.</exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available using the <see cref="System.InnerException">InnerException</see>  property. </exception>
        public static void LoadConfig()
        {
            LoadConfig(true);
        }
        /// <summary>
        /// This Loads the configeration file into the static feild LoginServer.XML.LoginServer.Instance.
        /// </summary>
        /// <exception cref="SharedServerLib.Exceptions.TortusMissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="SharedServerLib.Exceptions.TortusFileException">The configeration cannot be loaded or initally created.</exception>
        /// <exception cref="SharedServerLib.Exceptions.TortusFormatException">An IP or Hostname is invalid.</exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available using the <see cref="System.InnerException">InnerException</see>  property. </exception>
        public static void LoadConfig(bool ignoreErrors)
        {
            if (!File.Exists("./LoginConfig.xml"))
            {
                CreateDefault();
            }

            TextReader reader = new StreamReader("./LoginConfig.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(LoginServerConfig));
            LoginServerConfig._instance = (LoginServerConfig)serializer.Deserialize(reader);
            reader.Close();

            string[] AcceptedAddresses = LoginServerConfig.Instance.ServerListenAcceptedAddresses;
            int AddressLen = AcceptedAddresses.Length;

            LoginServerConfig.Instance.ConvertedAcceptedServerAddresses = new IPAddress[AddressLen];
            for (int Index = 0; Index < AddressLen; Index++)
            {
                if (!IPAddress.TryParse(AcceptedAddresses[Index], out LoginServerConfig.Instance.ConvertedAcceptedServerAddresses[Index]))
                {
                    //Maybe its a hostname
                    System.Net.IPAddress[] Addresses;
                    try
                    {
                        Addresses = Dns.GetHostEntry(AcceptedAddresses[Index]).AddressList;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (ignoreErrors)
                        {
                            Console.WriteLine("Value is not a valid IP Address or DNS Host: {0}", AcceptedAddresses[Index]);
                            continue;
                        }
                        else
                            throw new TortusFormatException("Value is not a valid IP Address or DNS Host", AcceptedAddresses[Index], "Any IP Address or DNS host");
                    }

                    if (Addresses.Length == 0)
                    {
                        if (ignoreErrors)
                        {
                            Console.WriteLine("DNS Host did not resolve to an IP address: {0}", AcceptedAddresses[Index]);
                            continue;
                        }
                        else
                            throw new TortusFormatException("DNS Host did not resolve to an IP address", AcceptedAddresses[Index], "Any IP Address or DNS host");
                    }

                    LoginServerConfig.Instance.ConvertedAcceptedServerAddresses[Index] = Addresses[0];
                }

                if (LoginServerConfig.Instance.ConvertedAcceptedServerAddresses[Index] == IPAddress.Any ||
                    LoginServerConfig.Instance.ConvertedAcceptedServerAddresses[Index] == IPAddress.IPv6Any)
                {
                    LoginServerConfig.Instance.AcceptAnyAddress = true;
                }


            }


            if (IPAddress.TryParse(LoginServerConfig.Instance.ClientListenAddress, out LoginServerConfig.Instance.ConvertedClientListenAddress))
            {
                throw new TortusFormatException("Value is not a valid IP Address or DNS Host", LoginServerConfig.Instance.ClientListenAddress, "Any IP Address or DNS host");
            }

            if (IPAddress.TryParse(LoginServerConfig.Instance.ServerListenAddress, out LoginServerConfig.Instance.ConvertedServerListenAddress))
            {
                throw new TortusFormatException("Value is not a valid IP Address or DNS Host", LoginServerConfig.Instance.ServerListenAddress, "Any IP Address or DNS host");
            }

        }
    }



}
