﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RocksmithSngCreator;

namespace RocksmithTookitGUI.SngFileCreator
{
    public enum ArrangementType { Instrument, Vocal }

    public partial class SngFileCreator : UserControl
    {
        public SngFileCreator()
        {
            InitializeComponent();
        }

        private string InputXmlFile
        {
            get { return inputXmlTextBox.Text; }
            set { inputXmlTextBox.Text = value; }
        }

        private string OutputSngFile
        {
            get { return outputFileTextBox.Text; }
            set { outputFileTextBox.Text = value; }
        }

        private ArrangementType ArrangementType
        {
            get
            {
                if (instrumentRadioButton.Checked)
                    return ArrangementType.Instrument;
                if (vocalsRadioButton.Checked)
                    return ArrangementType.Vocal;
                throw new InvalidOperationException("No arrangement type selected");
            }
        }

        private GamePlatform Platform
        {
            get
            {
                if (littleEndianRadioBtn.Checked)
                    return GamePlatform.Pc;
                if (bigEndianRadioBtn.Checked)
                    return GamePlatform.Console;
                throw new InvalidOperationException("No game platform selected");
            }
        }

        private void xmlBrowseButton_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "XML Files|*.xml|All Files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.ShowDialog();
                if (string.IsNullOrEmpty(fd.FileName)) return;

                InputXmlFile = fd.FileName;
                OutputSngFile = Path.ChangeExtension(fd.FileName, "sng");
            }
        }

        private void sngConvertButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputXmlFile)) return;
            if (string.IsNullOrEmpty(OutputSngFile)) return;

            try
            {
                var sngFileWriter = new SngFileWriter(Platform);
                if (ArrangementType == ArrangementType.Instrument)
                {
                    sngFileWriter.WriteRocksmithSongChart(InputXmlFile, OutputSngFile);
                }
                else
                {
                    sngFileWriter.WriteRocksmithVocalChart(InputXmlFile, OutputSngFile);
                }
                MessageBox.Show("Process Complete", "File Creation Process");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    errorMessage += "\n" + ex.InnerException.Message;

                PSTaskDialog.cTaskDialog.MessageBox("Error", "Conversion has failed.", errorMessage, ex.ToString(),
                    "Click 'show details' for complete exception information.", "",
                    PSTaskDialog.eTaskDialogButtons.OK, PSTaskDialog.eSysIcons.Error, PSTaskDialog.eSysIcons.Information);
            }
        }
    }
}
