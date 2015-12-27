using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    internal class SingletonLazyCreatorByFactory<TContract, TFactory> : SingletonLazyCreatorBase
        where TFactory : IFactory<TContract>
    {
        object _instance;

        public SingletonLazyCreatorByFactory(
            SingletonId id, SingletonProviderMap owner)
            : base(id, owner)
        {
        }

        public override object GetInstance(InjectContext context)
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = context.Container.Instantiate<TFactory>().Create();

            if (_instance == null)
            {
                throw new ZenjectResolveException(
                    "Failed to instantiate type '{0}' in SingletonLazyCreatorByFactory".Fmt(context.MemberType));
            }

            return _instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            if (typeof(TFactory).DerivesFrom<IValidatable>())
            {
                var factory = context.Container.Instantiate<TFactory>(context);
                return ((IValidatable)factory).Validate();
            }

            return Enumerable.Empty<ZenjectResolveException>();
        }
    }
}

