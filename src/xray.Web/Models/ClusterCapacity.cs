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
            double bufferPercentage,
            bool balancedBefore,
            bool balancedAfter,
            double deviationBefore,
            double deviationAfter,
            double balancingThreshold,
            string maxLoadedNode,
            string minLoadedNode
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
            this.BalancedBefore = balancedBefore;
            this.BalancedAfter = balancedAfter;
            this.DeviationBefore = deviationBefore;
            this.DeviationAfter = deviationAfter;
            this.BalancingThreshold = balancingThreshold;
            this.MaxLoadedNode = maxLoadedNode;
            this.MinLoadedNode = minLoadedNode;
        }

        public long BufferedCapacity { get; }

        public long Capacity { get; }

        public long Load { get; }

        public long RemainingBufferedCapacity { get; }

        public long RemainingCapacity { get; }

        public bool IsClusterCapacityViolation { get; }

        public string Name { get; }

        public double BufferPercentage { get; }

        public bool BalancedBefore { get; }

        public bool BalancedAfter { get; }

        public double DeviationBefore { get; }

        public double DeviationAfter { get; }

        public double BalancingThreshold { get; }

        public string MaxLoadedNode { get; }

        public string MinLoadedNode { get; }
    }
}