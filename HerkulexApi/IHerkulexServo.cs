using System.Collections.Generic;

namespace HerkulexApi
{
    public interface IHerkulexServo
    {
        int Id { get; }
        int NeutralPosition { get; set; }
        void MoveToNeutralPosition();
        void TorqueOn();
        void TorqueOff();
        void MoveServoPosition(double position, int playTime);
        bool Status();
        void Reboot();
        void SetColor(HerkulexColor color);
        void PlaySeries(IEnumerable<HerkulexDatapoint> targets);
        void ChangeBaudRate(HerkulexBaudRate baudRate);
        bool ChangeId(int newId);
        void AccelerationRatio(int ratio); 


    }
}
