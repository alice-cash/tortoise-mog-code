using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using System.Security;
using System.Diagnostics;

using Server.Exceptions;
using StormLib.XML;

namespace Server.XML
{
    /// <summary>
    /// configuration Database for Login Server
    /// </summary>
    [Serializable]
    [XmlRoot("Server")]
    public class LoginServerConfig: IConfig
    {
        private const string DefaultConfigPath = "Tortoise.Server.XML.DefaultConfig.xml";


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

        ///<summary>This is the Clients Major version number.</summary>
        public int ClientMajor;
        ///<summary> This is the Clients Minor version number. </summary>
        public int CLientMinor;
        ///<summary> This is the Clients Build version number. </summary>
        public int CLientBuild;
        ///<summary> This is the Clients Revision version number. </summary>
        public int ClientRevision;



        /// <summary>
        /// This creates a default Config and saves it.
        /// </summary>
        /// <exception cref="Server.Exceptions.MissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="Server.Exceptions.FileException">The configuration cannot be saved.</exception>
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
                throw new MissingResourceException("The resource could not be loaded.", DefaultConfigPath);
            }

            DefaultFile = SR.ReadToEnd();


            try
            {
                File.WriteAllText("./LoginConfig.xml", DefaultFile);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FileException(string.Format("The application does not have permission to save the file ./LoginConfig.xml"), "./LoginConfig.xml", ex);
            }
            catch (SecurityException ex)
            {
                throw new FileException(string.Format("The application does not have permission to save the file ./LoginConfig.xml"), "./LoginConfig.xml", ex);
            }
        }

        /// <summary>
        /// This Loads the configuration file into the static field Server.XML.Server.Instance.
        /// </summary>
        /// <exception cref="Server.Exceptions.MissingResourceException">The embedded resource cannot be loaded.</exception>
        /// <exception cref="Server.Exceptions.FileException">The configuration cannot be loaded or initially created.</exception>
        /// <exception cref="Server.Exceptions.FormatException">An IP or Hostname is invalid.</exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available using the <see cref="System.innerException">innerException</see>  property. </exception>
        public IConfig LoadConfig()
        {
            return LoadConfig(true);
        }
        /// <summary>
        /// This Loads the configuration file into the static feild Server.XML.Server.Instance.
        /// </summary>
        /// <exception cref="Server.Exceptions.MissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="Server.Exceptions.FileException">The configuration cannot be loaded or initally created.</exception>
        /// <exception cref="Server.Exceptions.ConfigFormatException">An IP or Hostname is invalid.</exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available using the <see cref="System.innerException">innerException</see>  property. </exception>
        public IConfig LoadConfig(bool ignoreErrors)
		{
			if (!File.Exists("./LoginConfig.xml"))
			{
				CreateDefault();
			}

			TextReader reader = new StreamReader("./LoginConfig.xml");

			XmlSerializer serializer = new XmlSerializer(typeof(LoginServerConfig));
			LoginServerConfig instance = (LoginServerConfig)serializer.Deserialize(reader);
			reader.Close();

			string[] AcceptedAddresses = instance.ServerListenAcceptedAddresses;
			int AddressLen = AcceptedAddresses.Length;

			instance.ConvertedAcceptedServerAddresses = new IPAddress[AddressLen];
			for (int Index = 0; Index < AddressLen; Index++)
			{
				if (!IPAddress.TryParse(AcceptedAddresses[Index], out instance.ConvertedAcceptedServerAddresses[Index]))
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
                            Trace.WriteLine("Value is not a valid IP Address or DNS Host: {0}", AcceptedAddresses[Index]);
							continue;
						}
						else
							throw new ConfigFormatException("Value is not a valid IP Address or DNS Host", AcceptedAddresses[Index], "Any IP Address or DNS host");
					}

					if (Addresses.Length == 0)
					{
						if (ignoreErrors)
						{
                            Trace.WriteLine("DNS Host did not resolve to an IP address: {0}", AcceptedAddresses[Index]);
							continue;
						}
						else
							throw new ConfigFormatException("DNS Host did not resolve to an IP address", AcceptedAddresses[Index], "Any IP Address or DNS host");
					}

					instance.ConvertedAcceptedServerAddresses[Index] = Addresses[0];
				}

				if (instance.ConvertedAcceptedServerAddresses[Index] == IPAddress.Any ||
					instance.ConvertedAcceptedServerAddresses[Index] == IPAddress.IPv6Any)
				{
					instance.AcceptAnyAddress = true;
				}


			}


            if (!IPAddress.TryParse(instance.ClientListenAddress, out instance.ConvertedClientListenAddress))
			{
				throw new ConfigFormatException("Value is not a valid IP Address or DNS Host", instance.ClientListenAddress, "Any IP Address or DNS host");
			}

			if (!IPAddress.TryParse(instance.ServerListenAddress, out instance.ConvertedServerListenAddress))
			{
				throw new ConfigFormatException("Value is not a valid IP Address or DNS Host", instance.ServerListenAddress, "Any IP Address or DNS host");
			}
            
            return instance;
		}

        public bool ValidateConfig(){
            if (MysqlUser == "{EDIT ME}" || MysqlPass == "{EDIT ME}")
            {
                Trace.WriteLine("Edit the LoginConfig.xml file");
                return false;
            }
            
            if (AcceptAnyAddress)
            {
                Trace.WriteLine("Warning: This server is set to accept server connections from any IP address. ANY server with the secrets can connect.");
            }
            return true;
        }


    }



}
