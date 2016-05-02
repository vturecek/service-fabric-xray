// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Xray.Models
{
    public struct ClusterCapacity
    {
        public ClusterCapacity(
            string name,
            long bufferedCapacity,
            long capacity,
            long load,
            long remainingBufferedCapacity,
            long remainingCapacity,
            bool isViolation,
            double bufferPercentage
            )
        {
            this.Name = name;
            this.BufferedCapacity = bufferedCapacity;
            this.Capacity = capacity;
            this.Load = load;
            this.RemainingBufferedCapacity = remainingBufferedCapacity;
            this.RemainingCapacity = remainingCapacity;
            this.IsClusterCapacityViolation = isViolation;
            this.BufferPercentage = bufferPercentage;
        }

        public long BufferedCapacity { get; }

        public long Capacity { get; }

        public long Load { get; }

        public long RemainingBufferedCapacity { get; }

        public long RemainingCapacity { get; }

        public bool IsClusterCapacityViolation { get; }

        public string Name { get; }

        public double BufferPercentage { get; }
    }
}