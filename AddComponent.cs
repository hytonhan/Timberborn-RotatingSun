using Bindito.Unity;
using Timberborn.Fields;
using TimberbornAPI.EntityActionSystem;
using UnityEngine;

namespace SunFix
{
    /// <summary>
    /// Add Custom Sunflower class for Sunflowers
    /// </summary>
    public class AddComponent : IEntityAction
    {
        private readonly IInstantiator _instantiator;

        public AddComponent(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }


        public void ApplyToEntity(GameObject entity)
        {
            var component = entity.GetComponent<Crop>();
            if (component == null || !component.name.Contains("Sun"))
                return;

            _instantiator.AddComponent<RotatingSunflower>(entity);
        }
    }
}
