// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Facilities.Remoting
{
#if (!SILVERLIGHT)
	using System;
	using System.IO;
	using System.Runtime.Remoting;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	///   Facility to allow the communication with remote kernel, using the .NET Remoting infraestructure.
	/// </summary>
	/// <remarks>
	///   TODO
	/// </remarks>
	/// <example>
	///   TODO
	/// </example>
	public class RemotingFacility : AbstractFacility
	{
		/// <summary>
		///   Used for client side (Expand explanation)
		/// </summary>
		private String baseUri;

		private ITypeConverter converter;

		private bool disconnectLocalRegistry;
		private bool isClient;
		private bool isServer;

		/// <summary>
		///   Used for server side. 
		///   Holds the local registry
		/// </summary>
		private RemotingRegistry localRegistry;

		/// <summary>
		///   Used for client side. 
		///   Holds a remote proxy to the server registry
		/// </summary>
		private RemotingRegistry remoteRegistry;

		/// <summary>
		///   Performs the tasks associated with freeing, releasing, or resetting
		///   the facility resources.
		/// </summary>
		/// <remarks>
		///   It can be overriden.
		/// </remarks>
		public override void Dispose()
		{
			if (disconnectLocalRegistry)
			{
				RemotingServices.Disconnect(localRegistry);
			}

			base.Dispose();
		}

		protected override void Init()
		{
			ObtainConverter();

			SetUpRemotingConfiguration();

			baseUri = FacilityConfig.Attributes["baseUri"];

			var conversionManager = Kernel.GetConversionManager();
			if (conversionManager.PerformConversion<bool?>(FacilityConfig.Attributes["isServer"]) == true)
			{
				isServer = true;
				ConfigureServerFacility();
			}

			if (conversionManager.PerformConversion<bool?>(FacilityConfig.Attributes["isClient"]) == true)
			{
				isClient = true;
				ConfigureClientFacility();
			}

			Kernel.ComponentModelBuilder.AddContributor(
				new RemotingInspector(converter, isServer, isClient, baseUri, remoteRegistry, localRegistry));
		}

		private void ConfigureClientFacility()
		{
			var remoteKernelUri = FacilityConfig.Attributes["remoteKernelUri"];

			if (remoteKernelUri == null || remoteKernelUri.Length == 0)
			{
				var message = "When the remote facility is configured as " +
				              "client you must supply the URI for the kernel using the attribute 'remoteKernelUri'";

				throw new Exception(message);
			}

			remoteRegistry = (RemotingRegistry)
			                 RemotingServices.Connect(typeof(RemotingRegistry), remoteKernelUri);
		}

		private void ConfigureServerFacility()
		{
			Kernel.Register(Component.For<RemotingRegistry>().Named("remoting.registry"));

			localRegistry = Kernel.Resolve<RemotingRegistry>();

			var kernelUri = FacilityConfig.Attributes["registryUri"];

			if (string.IsNullOrEmpty(kernelUri))
			{
				var message = "When the remote facility is configured as " +
				              "server you must supply the URI for the component registry using the attribute 'registryUri'";

				throw new Exception(message);
			}

			RemotingServices.Marshal(localRegistry, kernelUri, typeof(RemotingRegistry));

			disconnectLocalRegistry = true;
		}

		private void ObtainConverter()
		{
			converter = (ITypeConverter)Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
		}

		private void SetUpRemotingConfiguration()
		{
			var configurationFile = FacilityConfig.Attributes["remotingConfigurationFile"];

			if (configurationFile == null)
			{
				return;
			}

			if (!Path.IsPathRooted(configurationFile))
			{
				configurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationFile);
			}

			if (!File.Exists(configurationFile))
			{
				var message = String.Format("Remoting configuration file '{0}' does not exist", configurationFile);

				throw new Exception(message);
			}

#if !MONO
			RemotingConfiguration.Configure(configurationFile, false);
#else
			RemotingConfiguration.Configure(configurationFile);
#endif
		}
	}
#endif
}