using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerkulexApi
{
    interface IServo
    {
        int Id { get; }
        int NeutralPosition { get; set; }
        void MoveToNeutralPosition();
        void TorqueOn();
        void TorqueOff();
        void MoveServoPosition(double position, int playTime);
        bool Status();
        void Reboot(); 

    }
}
