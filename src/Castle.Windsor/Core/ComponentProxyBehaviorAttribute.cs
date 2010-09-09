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

namespace Castle.Core
{
	using System;
	using System.ComponentModel;

	/// <summary>
	/// Specifies the proxying behavior for a component.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ComponentProxyBehaviorAttribute : Attribute
	{
		private Type[] additionalInterfaces;

#if !(SILVERLIGHT)
		/// <summary>
		/// Gets or sets a value indicating whether the generated 
		/// interface proxy should inherit from <see cref="MarshalByRefObject"/>.
		/// </summary>
		public bool UseMarshalByRefProxy { get; set; }

#endif

		/// <summary>
		/// Determines if the component requires a single interface proxy.
		/// </summary>
		/// <value><c>true</c> if the component requires a single interface proxy.</value>
		[Obsolete("Prefer using a IProxyGenerationHook.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool UseSingleInterfaceProxy { get; set; }

		/// <summary>
		///  Gets or sets the additional interfaces used during proxy generation.
		/// </summary>
		public Type[] AdditionalInterfaces
		{
			get
			{
				if (additionalInterfaces != null)
				{
					return additionalInterfaces;
				}

				return Type.EmptyTypes;
			}
			set { additionalInterfaces = value; }
		}
	}
}