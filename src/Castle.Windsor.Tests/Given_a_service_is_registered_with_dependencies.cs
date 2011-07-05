using System.Diagnostics;

namespace Castle.Windsor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Core;
    using NUnit.Framework;

    public class Given_a_service_is_registered_with_dependencies
    {
        private class Class1
        {}

        private class DependentOnClass1
        {
            private readonly Class1 _dependency;

            public DependentOnClass1(Class1 dependency)
            {
                _dependency = dependency;
            }
        }

        [Test]
        public void When_registering_services_on_multiple_threads_it_should_not_throw_exceptions()
        {
            IWindsorContainer container = new WindsorContainer();

            var threads = new Dictionary<ThreadStart, IAsyncResult>();

            for (int i = 0; i < 500; i++)
            {
                var key = Guid.NewGuid().ToString();
                var key2 = Guid.NewGuid().ToString();
                ThreadStart t1 = () =>
                {
                    container.AddComponentLifeStyle(key, typeof(DependentOnClass1), typeof(DependentOnClass1), LifestyleType.Transient);

                    Thread.Sleep(100);

                    container.AddComponentLifeStyle(key2, typeof(Class1), typeof(Class1), LifestyleType.Transient);
                };

                var r1 = t1.BeginInvoke(null, null);
                threads.Add(t1, r1);
            }

            var exceptionCount = 0;
            foreach (var kvp in threads)
            {
                try
                {
                    kvp.Key.EndInvoke(kvp.Value);
                }
                catch
                {
                    exceptionCount++;
                }
            }

            Debug.Print("{0} exceptions raised.", exceptionCount);
            Assert.AreEqual(0, exceptionCount);
        }
    }
}