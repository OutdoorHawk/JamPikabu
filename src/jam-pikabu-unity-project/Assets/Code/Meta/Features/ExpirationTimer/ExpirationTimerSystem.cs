using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Common.Time;
using Code.Infrastructure.Systems;
using Entitas;

namespace Code.Meta.Features.ExpirationTimer
{
    public class ExpirationTimerSystem : TimerExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<MetaEntity> _entities;
        private readonly IGroup<MetaEntity> _expired;
        private readonly List<MetaEntity> _buffer = new(32);

        public ExpirationTimerSystem(float executeIntervalSeconds, ITimeService time, MetaContext context) : base(executeIntervalSeconds, time)
        {
            _entities = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.ExpirationTime)
                .NoneOf(
                    MetaMatcher.Expired));
            
            _expired = context.GetGroup(MetaMatcher
                .AllOf(
                    MetaMatcher.Expired));
        }

        protected override void Execute()
        {
            foreach (var entity in _entities.GetEntities(_buffer))
            {
                float diff = (entity.ExpirationTime - _time.TimeStamp).ZeroIfNegative();
                
                if (diff <= 0)
                {
                    entity.isExpired = true;
                }
            }
        }

        public void Cleanup()
        {
            foreach (MetaEntity expired in _expired)
            {
                expired.isDestructed = true;
            }
        }
    }
}