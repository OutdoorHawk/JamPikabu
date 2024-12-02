using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entitas;

namespace Code.Infrastructure.Systems
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public abstract class BufferedExecuteSystem : IExecuteSystem
    {
        protected readonly List<GameEntity> _buffer;

        protected virtual int BufferCapacity => 2;
        
        protected BufferedExecuteSystem()
        {
            _buffer = new List<GameEntity>(BufferCapacity);
        }

        public abstract void Execute();
    }
}