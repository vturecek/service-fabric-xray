// ------------------------------------------------------------
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace xray.Models
{
    public struct ClusterNodeCapacity
    {
        public ClusterNodeCapacity(
            string name,
            bool isViolation,
            long bufferedCapacity,
            long capacity,
            long load,
            long remainingBufferedCapacity,
            long remainingCapacity)
        {
            this.Name = name;
            this.IsCapacityViolation = isViolation;
            this.BufferedCapacity = bufferedCapacity;
            this.Capacity = capacity;
            this.Load = load;
            this.RemainingBufferedCapacity = remainingBufferedCapacity;
            this.RemainingCapacity = remainingCapacity;
        }

        public bool IsCapacityViolation { get; }

        public string Name { get; }

        public long BufferedCapacity { get; }

        public long Capacity { get; }

        public long Load { get; }

        public long RemainingBufferedCapacity { get; }

        public long RemainingCapacity { get; }
    }
}