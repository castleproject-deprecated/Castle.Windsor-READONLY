﻿// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Experimental.Debugging.Extensions
{
	using System.Collections.Generic;
	using System.Linq;

	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Naming;
	using Castle.Windsor.Experimental.Debugging.Primitives;

	public class PotentiallyMisconfiguredComponents : AbstractContainerDebuggerExtension
	{
		private INamingSubSystem naming;

		public override IEnumerable<DebuggerViewItem> Attach()
		{
			var waitingComponents = naming.GetKey2Handler()
				.Where(h => h.Value.CurrentState == HandlerState.WaitingDependency);
			if (waitingComponents.Any() == false)
			{
				yield break;
			}
			var waiting = GetMetaComponents(waitingComponents.ToDictionary(k => k.Key, v => v.Value));
			var components = waiting.Select(DefaultComponentView).ToArray();
			yield return new DebuggerViewItem("Potentially Misconfigured Components",
			                                  "Count = " + components.Length,
			                                  components);
		}

		public override void Init(IKernel kernel)
		{
			naming = kernel.GetSubSystem(SubSystemConstants.NamingKey) as INamingSubSystem;
		}
	}
}