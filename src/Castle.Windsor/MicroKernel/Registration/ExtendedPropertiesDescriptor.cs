// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Registration
{
	using System.Collections;
	using Castle.Core;

	public class ExtendedPropertiesDescriptor<S> : AbstractPropertyDescriptor<S>
	{
		public ExtendedPropertiesDescriptor(params Property[] properties)
			: base(properties)
		{
		}

		public ExtendedPropertiesDescriptor(IDictionary dictionary)
			: base(dictionary)
		{
		}

		public ExtendedPropertiesDescriptor(object overridesAsAnonymousType)
			: base(new ReflectionBasedDictionaryAdapter(overridesAsAnonymousType))
		{
		}

		protected override void ApplyProperty(IKernel kernel, ComponentModel model, object key, object value, Property property)
		{
			model.ExtendedProperties[key] = value;
		}
	}
}