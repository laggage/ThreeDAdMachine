namespace Communication.Models
{
    public struct DeviceCommand
    {
        public DeviceCommand(string commandName, int commandCode)
        {
            if (string.IsNullOrEmpty(commandName))
                commandName = "Untitled Command";
            CommandName = commandName;
            CommandCode = commandCode;
            _hashCode = CommandName.GetHashCode() ^ CommandCode.GetHashCode();
        }
        /// <summary>
        /// 散列码缓存
        /// </summary>
        private readonly int _hashCode;
        /// <summary>
        /// 设备命令代码
        /// </summary>
        public int CommandCode { get; set; }
        /// <summary>
        /// 设备命令名称
        /// </summary>
        public string CommandName { get; set; }

        public override string ToString() => CommandName;

        public override int GetHashCode() => _hashCode;

        public bool Equals(DeviceCommand obj) => obj.CommandCode == CommandCode;

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;
            return Equals((DeviceCommand)obj);
        }

        public static bool operator ==(DeviceCommand left, DeviceCommand right) => left.Equals(right);

        public static bool operator !=(DeviceCommand left, DeviceCommand right) => !(left == right);
    }
}
