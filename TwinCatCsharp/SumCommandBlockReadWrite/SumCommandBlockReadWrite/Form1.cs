﻿/*
 * 
 * Function: Read or write several variables in one command
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TwinCAT.Ads;

namespace SumCommandBlockReadWrite
{    
    // Structure declaration for values
    internal struct MyStruct
    {
        public ushort uintValue;
        public int dintValue;
        public bool boolValue;
    }

    // Structure declaration for handles
    internal struct VariableInfo
    {
        public int indexGroup;
        public int indexOffset;
        public int length;
    }
    public partial class Form1 : Form
    {
        private TcAdsClient adsClient;
        private string[] variableNames;
        private int[] variableLengths;
        VariableInfo[] variables;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Connect to PLC
                adsClient = new TcAdsClient();
                adsClient.Connect(851);

                // Fill structures with name and size of PLC variables
                variableNames = new string[] { "MAIN.uintValue", "MAIN.dintValue", "MAIN.boolValue" };
                variableLengths = new int[] { 2, 4, 1 };

                // Write handle parameter into structure
                variables = new VariableInfo[variableNames.Length];
                for (int i = 0; i < variables.Length; i++)
                {
                    variables[i].indexGroup = (int)AdsReservedIndexGroups.SymbolValueByHandle;
                    variables[i].indexOffset = adsClient.CreateVariableHandle(variableNames[i]);
                    variables[i].length = variableLengths[i];
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                adsClient = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (adsClient == null)
                return;

            try
            {
                // Get the ADS return codes and examine for errors
                BinaryReader reader = new BinaryReader(BlockRead(variables));
                for (int i = 0; i < variables.Length; i++)
                {
                    int error = reader.ReadInt32();
                    if (error != (int)AdsErrorCode.NoError)
                        System.Diagnostics.Debug.WriteLine(
                            String.Format("Unable to read variable {0} (Error = {1})", i, error));
                }

                // Read the data from the ADS stream
                MyStruct myStruct;
                myStruct.uintValue = reader.ReadUInt16();
                myStruct.dintValue = reader.ReadInt32();
                myStruct.boolValue = reader.ReadBoolean();

                // Write data from the structure into the text boxes
                tbUint.Text = myStruct.uintValue.ToString();
                tbDint.Text = myStruct.dintValue.ToString();
                tbBool.Text = myStruct.boolValue.ToString();
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
        private AdsStream BlockRead(VariableInfo[] variables)
        {
            // Allocate memory
            int rdLength = variables.Length * 4;
            int wrLength = variables.Length * 12;

            // Write data for handles into the ADS Stream
            BinaryWriter writer = new BinaryWriter(new AdsStream(wrLength));
            for (int i = 0; i < variables.Length; i++)
            {
                writer.Write(variables[i].indexGroup);
                writer.Write(variables[i].indexOffset);
                writer.Write(variables[i].length);
                rdLength += variables[i].length;
            }

            // Sum command to read variables from the PLC
            AdsStream rdStream = new AdsStream(rdLength);
            adsClient.ReadWrite(0xF080, variables.Length, rdStream, (AdsStream)writer.BaseStream);

            // Return the ADS error codes
            return rdStream;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (adsClient == null)
                return;
            try
            {
                // Get the ADS return codes and examine for errors
                BinaryReader reader = new BinaryReader(BlockRead2(variables));
                for (int i = 0; i < variables.Length; i++)
                {
                    int error = reader.ReadInt32();
                    if (error != (int)AdsErrorCode.NoError)
                        System.Diagnostics.Debug.WriteLine(
                            String.Format("Unable to read variable {0} (Error = {1})", i, error));
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
        private AdsStream BlockRead2(VariableInfo[] variables)
        {
            // Allocate memory
            int rdLength = variables.Length * 4;
            int wrLength = variables.Length * 12 + 7;

            BinaryWriter writer = new BinaryWriter(new AdsStream(wrLength));
            MyStruct myStruct;
            myStruct.uintValue = ushort.Parse(tbUint2.Text);
            myStruct.dintValue = int.Parse(tbDint2.Text);
            myStruct.boolValue = bool.Parse(tbBool2.Text);

            // Write data for handles into the ADS stream
            for (int i = 0; i < variables.Length; i++)
            {
                writer.Write(variables[i].indexGroup);
                writer.Write(variables[i].indexOffset);
                writer.Write(variables[i].length);
            }

            // Write data to send to PLC behind the structure
            writer.Write(myStruct.uintValue);
            writer.Write(myStruct.dintValue);
            writer.Write(myStruct.boolValue);

            // Sum command to write the data into the PLC
            AdsStream rdStream = new AdsStream(rdLength);
            adsClient.ReadWrite(0xF081, variables.Length, rdStream, (AdsStream)writer.BaseStream);

            // Return the ADS error codes
            return rdStream;
        }
    }
}
