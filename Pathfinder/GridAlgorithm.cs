using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    interface GridAlgorithm
    {
        void continueSearch();
        void setStopFlag(bool stopFlag);
        void setStepDelay(int stepDelay);
        void drawPath(FieldItem startPoint, FieldItem targetPoint, bool diagAllowed);
    }
}
