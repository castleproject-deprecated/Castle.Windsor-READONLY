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

namespace Castle.MicroKernel.Handlers
{
	using System;

	using Castle.Core;
	using Castle.MicroKernel.Context;

	public class ForwardingHandler : IHandler
	{
		private readonly Type forwardedType;
		private readonly IHandler target;

		public ForwardingHandler(IHandler target, Type forwardedType)
		{
			this.target = target;
			this.forwardedType = forwardedType;
		}

		public IHandler Target
		{
			get { return target; }
		}

		public ComponentModel ComponentModel
		{
			get { return target.ComponentModel; }
		}

		public HandlerState CurrentState
		{
			get { return target.CurrentState; }
		}

		public Type Service
		{
			get { return forwardedType; }
		}

		public void AddCustomDependencyValue(object key, object value)
		{
			target.AddCustomDependencyValue(key, value);
		}

		public bool HasCustomParameter(object key)
		{
			return target.HasCustomParameter(key);
		}

		public void Init(IKernel kernel)
		{
			Target.Init(kernel);
		}

		public bool IsBeingResolvedInContext(CreationContext context)
		{
			return context.IsInResolutionContext(this) || target.IsBeingResolvedInContext(context);
		}

		public bool Release(object instance)
		{
			return target.Release(instance);
		}

		public void RemoveCustomDependencyValue(object key)
		{
			target.RemoveCustomDependencyValue(key);
		}

		public object Resolve(CreationContext context)
		{
			return target.Resolve(context);
		}

		public object TryResolve(CreationContext context)
		{
			return target.TryResolve(context);
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
		                       ComponentModel model, DependencyModel dependency)
		{
			return target.CanResolve(context, contextHandlerResolver, model, dependency);
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
		                      ComponentModel model, DependencyModel dependency)
		{
			return target.Resolve(context, contextHandlerResolver, model, dependency);
		}

		public event HandlerStateDelegate OnHandlerStateChanged
		{
			add { target.OnHandlerStateChanged += value; }
			remove { target.OnHandlerStateChanged -= value; }
		}
	}
}