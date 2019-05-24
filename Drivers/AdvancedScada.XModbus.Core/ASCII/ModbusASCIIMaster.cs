﻿using AdvancedScada.DriverBase;
using AdvancedScada.DriverBase.Comm;
using HslCommunication;
using HslCommunication.ModBus;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms;

namespace AdvancedScada.XModbus.Core.ASCII
{
    public class ModbusASCIIMaster : IDriverAdapter
    {
          private SerialPort serialPort;

        public bool IsConnected { get; set; }
        public byte Station { get; set; }
        public ModbusASCIIMaster(short slaveId, SerialPort serialPort)
        {
            Station = (byte)slaveId;
            this.serialPort = serialPort;
        }

        private ModbusAscii busAsciiClient = null;

        public void Connection()
        {
           
            busAsciiClient?.Close();
            busAsciiClient = new ModbusAscii(Station);
            busAsciiClient.AddressStartWithZero = true;

            busAsciiClient.IsStringReverse = false;
            try
            {

                busAsciiClient.SerialPortInni(sp =>
                {
                    sp.PortName = serialPort.PortName;
                    sp.BaudRate = serialPort.BaudRate;
                    sp.DataBits = serialPort.DataBits;
                    sp.StopBits = serialPort.StopBits;
                    sp.Parity = serialPort.Parity;
                });
                busAsciiClient.Open();
                IsConnected = true;
                var err = new HMIException.ScadaException(IsConnected);
             
            }
            catch (TimeoutException ex)
            {
               

                var err = new HMIException.ScadaException(this.GetType().Name,
                    $"Could Not Connect to Server : {ex.Message}");
                var err1 = new HMIException.ScadaException(false);
            }
        }

        public void Disconnection()
        {
            try
            {
                busAsciiClient.Close();
                var err = new HMIException.ScadaException(false);
            }
            catch (TimeoutException ex)
            {

                var err = new HMIException.ScadaException(this.GetType().Name, $"Could Not Connect to Server : {ex.Message}");
                var err1 = new HMIException.ScadaException(false);
            }
        }


         
        public byte[] BuildReadByte(byte station, string address, ushort length)
        {
            var frame = DemoUtils.BulkReadRenderResult(busAsciiClient, address, length);


            return frame;
        }

        public byte[] BuildWriteByte(byte station, string address, byte[] value)
        {
            try
            {
                DemoUtils.WriteResultRender(busAsciiClient.Write(address, value), address);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return new byte[0];
        }

     

        public OperateResult<bool[]> ReadDiscrete(string address, ushort length)
        {
            return busAsciiClient.ReadDiscrete(address, length);
        }

        public bool Write(string address, dynamic value)
        {
            if (value is bool)
            {
                busAsciiClient.WriteCoil(address, value);
            }
            else
            {
                busAsciiClient.Write(address, value);
            }

            return true;
        }

        public TValue[] Read<TValue>(string address, ushort length)
        {
            if (typeof(TValue) == typeof(bool))
            {
                var b = busAsciiClient.ReadCoil(address, length).Content;
                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(ushort))
            {
                var b = busAsciiClient.ReadUInt16(address, length).Content;

                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(int))
            {
                var b = busAsciiClient.ReadInt32(address, length).Content;

                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(uint))
            {
                var b = busAsciiClient.ReadUInt32(address, length).Content;
                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(long))
            {
                var b = busAsciiClient.ReadInt64(address, length).Content;
                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(ulong))
            {
                var b = busAsciiClient.ReadUInt64(address, length).Content;
                return (TValue[])(object)b;
            }

            if (typeof(TValue) == typeof(short))
            {
                var b = busAsciiClient.ReadInt16(address, length).Content;
                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(double))
            {
                var b = busAsciiClient.ReadDouble(address, length).Content;
                return (TValue[])(object)b;
            }
            if (typeof(TValue) == typeof(float))
            {
                var b = busAsciiClient.ReadFloat(address, length).Content;
                return (TValue[])(object)b;

            }
            if (typeof(TValue) == typeof(string))
            {
                var b = busAsciiClient.ReadString(address, length).Content;
                return (TValue[])(object)b;
            }

            throw new InvalidOperationException(string.Format("type '{0}' not supported.", typeof(TValue)));
        }
    }
}