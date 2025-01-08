using UnityEngine;

namespace Code.Gameplay.Features.Distraction.Factory
{
    public interface IDistractionObjectsFactory
    {
        void CreateDistractionObject(DistractionObjectTypeId typeId, Transform at);
    }
}