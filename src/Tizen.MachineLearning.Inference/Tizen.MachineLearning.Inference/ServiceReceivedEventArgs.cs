using System;

namespace Tizen.MachineLearning.Inference
{
    public class ServiceReceivedEventArgs : EventArgs
    {
        internal ServiceReceivedEventArgs(MlInformation info, TensorsData data)
        {
            Info = info;
            Data = data;
        }

        public MlInformation Info { get; }
        public TensorsData Data { get; }
    }
}
