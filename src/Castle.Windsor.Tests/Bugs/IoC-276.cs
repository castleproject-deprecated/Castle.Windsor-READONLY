using NUnit.Framework;

namespace Castle.Windsor.Tests.Bugs
{
	using Castle.Core;
	using Castle.MicroKernel.Registration;

	[TestFixture]
	public class IoC_276
	{
		[Test]
		public void can_use_configure_when_using_multiple_based_on_descriptors()
		{
			using (var container = new WindsorContainer())
			{
				container.Register(AllTypes.FromAssemblyContaining<SomeService>()
				                   	.BasedOn<A>()
				                   	.BasedOn<B>()
				                   	.Configure(registration => registration.LifeStyle.Transient
				                   	                           	.Named(registration.Implementation.Name.ToLowerInvariant())));

				var handler = container.Kernel.GetHandler(typeof(SomeService));

				Assert.AreEqual("someservice", handler.ComponentModel.Name);
				Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);

			}
		}

		public interface A{}
		public interface B{}

		public class SomeService : A,B{}
	}


}
