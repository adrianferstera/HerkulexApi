
namespace HerkulexApi
{
   public enum HerkulexCmd
    {
        EEP_WRITE_REQ = 0x01,
        EEP_READ_REQ = 0x02,
        RAM_WRITE_REQ = 0x03,
        RAM_READ_REQ = 0x04,
        I_JOG_REQ = 0x05,
        S_JOG_REQ = 0x06,
        STAT_REQ = 0x07,
        ROLLBACK_REQ = 0x08,
        REBOOT_REQ = 0x09
    }

    public enum HerkulexBaudRate
    {
         DEFAULT115200 = 0x10,
         RATE57600 = 0x22,
         RATE1000000 = 0x01, 
         RATE666666 = 0x02, 
         RATE500000 = 0x03, 
         RATE400000=0x04, 
         RATE250000 = 0x07, 
         RATE200000 = 0x09
    }

    public enum HerkulexColor
    {
        NO_COLOR = 0x00, 
        GREEN = 0x01, 
        BLUE = 0x02, 
        RED = 0x04
    }



    public enum ACKPackage
    {
        EEP_WRITE_ACK = 0x41,
        EEP_READ_ACK = 0x42,
        RAM_WRITE_ACK = 0x43,
        RAM_READ_ACK = 0x44,
        I_JOG_ACK = 0x45,
        S_JOG_ACK = 0x46,
        STAT_ACK = 0x47,
        ROLLBACK_ACK = 0x48,
        REBOOT_ACK = 0x49,
    }

    public enum Torque
    {
        RAM = 52, 
        ON = 0x60, 
        OFF = 0x00, 
        BREAK=0x40
    }

    public enum Ram
    {
        MODEL_NO1_EEP = 0,
        MODEL_NO2_EEP = 1,
        VERSION1_EEP = 2,
        VERSION2_EEP = 3,
        BAUD_RATE_EEP = 4,
        SERVO_ID_EEP = 6,
        SERVO_ID_RAM = 0,
        ACK_POLICY_EEP = 7,
        ACK_POLICY_RAM = 1,
        ALARM_LED_POLICY_EEP = 8,
        ALARM_LED_POLICY_RAM = 2,
        TORQUE_POLICY_EEP = 9,
        TORQUE_POLICY_RAM = 3,
        MAX_TEMP_EEP = 11,
        MAX_TEMP_RAM = 5,
        MIN_VOLTAGE_EEP = 12,
        MIN_VOLTAGE_RAM = 6,
        MAX_VOLTAGE_EEP = 13,
        MAX_VOLTAGE_RAM = 7,
        ACCELERATION_RATIO_EEP = 14,
        ACCELERATION_RATIO_RAM = 8,
        MAX_ACCELERATION_TIME_EEP = 15,
        MAX_ACCELERATION_TIME_RAM = 9,
        DEAD_ZONE_EEP = 16,
        DEAD_ZONE_RAM = 10,
        SATURATOR_OFFSET_EEP = 17,
        SATURATOR_OFFSET_RAM = 11,
        SATURATOR_SLOPE_EEP = 18,
        SATURATOR_SLOPE_RAM = 12,
        PWM_OFFSET_EEP = 20,
        PWM_OFFSET_RAM = 14,
        MIN_PWM_EEP = 21,
        MIN_PWM_RAM = 15,
        MAX_PWM_EEP = 22,
        MAX_PWM_RAM = 16,
        OVERLOAD_PWM_THRESHOLD_EEP = 24,
        OVERLOAD_PWM_THRESHOLD_RAM = 18,
        MIN_POSITION_EEP = 26,
        MIN_POSITION_RAM = 20,
        MAX_POSITION_EEP = 28,
        MAX_POSITION_RAM = 22,
        POSITION_KP_EEP = 30,
        POSITION_KP_RAM = 24,
        POSITION_KD_EEP = 32,
        POSITION_KD_RAM = 26,
        POSITION_KI_EEP = 34,
        POSITION_KI_RAM = 28,
        POSITION_FEEDFORWARD_GAIN1_EEP = 36,
        POSITION_FEEDFORWARD_GAIN1_RAM = 30,
        POSITION_FEEDFORWARD_GAIN2_EEP = 38,
        POSITION_FEEDFORWARD_GAIN2_RAM = 32,
        VELOCITY_KP_EEP = 40,
        VELOCITY_KP_RAM = 34,
        VELOCITY_KI_EEP = 42,
        VELOCITY_KI_RAM = 36,
        LED_BLINK_PERIOD_EEP = 44,
        LED_BLINK_PERIOD_RAM = 38,
        ADC_FAULT_CHECK_PERIOD_EEP = 45,
        ADC_FAULT_CHECK_PERIOD_RAM = 39,
        PACKET_GARBAGE_CHECK_PERIOD_EEP = 46,
        PACKET_GARBAGE_CHECK_PERIOD_RAM = 40,
        STOP_DETECTION_PERIOD_EEP = 47,
        STOP_DETECTION_PERIOD_RAM = 41,
        OVERLOAD_DETECTION_PERIOD_EEP = 48,
        OVERLOAD_DETECTION_PERIOD_RAM = 42,
        STOP_THRESHOLD_EEP = 49,
        STOP_THRESHOLD_RAM = 43,
        INPOSITION_MARGIN_EEP = 50,
        INPOSITION_MARGIN_RAM = 44,
        CALIBRATION_DIFF_LOW_EEP = 52,
        CALIBRATION_DIFF_LOW_RAM = 46,
        CALIBRATION_DIFF_UP_EEP = 53,
        CALIBRATION_DIFF_UP_RAM = 47,
        STATUS_ERROR_RAM = 48,
        STATUS_DETAIL_RAM = 49,
        AUX1_RAM = 50,
        TORQUE_CONTROL_RAM = 52,
        LED_CONTROL_RAM = 53,
        VOLTAGE_RAM = 54,
        TEMPERATURE_RAM = 55,
        CURRENT_CONTROL_MODE_RAM = 56,
        TICK_RAM = 57,
        CALIBRATED_POSITION_RAM = 58,
        ABSOLUTE_POSITION_RAM = 60,
        DIFFERENTIAL_POSITION_RAM = 62,
        PWM_RAM = 64,
        ABSOLUTE_SECOND_POSITION_RAM = 66,
        ABSOLUTE_GOAL_POSITION_RAM = 68,
        ABSOLUTE_DESIRED_TRAJECTORY_POSITION = 70,
        DESIRED_VELOCITY_RAM = 72, 
        START_SEQUENCE = 255
    }

         
    /*
        public static byte BYTE1 = 0x01;
        public static byte BYTE2 = 0x02;

        public static byte BROADCAST_ID = 0xFE;*/
    }

