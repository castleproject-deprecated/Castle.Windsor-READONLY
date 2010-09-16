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

namespace Castle.Windsor.Tests.Facilities
{
	using Castle.Core.Configuration;
	using Castle.Facilities.Startable;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Tests.ClassComponents;

	using NUnit.Framework;

	[TestFixture]
	public class FacilityTestCase
	{
		private const string FacilityKey = "testFacility";
		private HiperFacility facility;
		private IKernel kernel;

		[Test]
		public void Creation()
		{
			var facility = kernel.GetFacilities()[0];

			Assert.IsNotNull(facility);
			Assert.AreSame(this.facility, facility);
		}

		[SetUp]
		public void Init()
		{
			kernel = new DefaultKernel();

			IConfiguration confignode = new MutableConfiguration("facility");
			IConfiguration facilityConf = new MutableConfiguration(FacilityKey);
			confignode.Children.Add(facilityConf);
			kernel.ConfigurationStore.AddFacilityConfiguration(FacilityKey, confignode);

			facility = new HiperFacility();

			Assert.IsFalse(facility.Initialized);
			kernel.AddFacility(FacilityKey, facility);
		}

		[Test]
		public void LifeCycle()
		{
			Assert.IsFalse(this.facility.Terminated);

			var facility = kernel.GetFacilities()[0];

			Assert.IsTrue(this.facility.Initialized);
			Assert.IsFalse(this.facility.Terminated);

			kernel.Dispose();

			Assert.IsTrue(this.facility.Initialized);
			Assert.IsTrue(this.facility.Terminated);
		}

		[Test]
		public void OnCreationCallback()
		{
			StartableFacility facility = null;

			kernel.AddFacility<StartableFacility>(f => facility = f);

			Assert.IsNotNull(facility);
		}
	}
}