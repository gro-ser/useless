using System;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace useless
{
    public partial class Speaker : Form
    {
        private void WriteLine(string str) => textBox1.AppendText(str+"\r\n");

        private readonly VoiceHints voiceHints = new Speaker.VoiceHints();
        private readonly System.Speech.Synthesis.SpeechSynthesizer synth = new SpeechSynthesizer();

        public Speaker() => InitializeComponent();

        private void Speaker_Load(object sender, EventArgs e)
        {
            /*synth.VoiceChange += Synth_VoiceChange;
            Synth_VoiceChange(null, null);*/
            propertyGrid1.SelectedObject = voiceHints;
            foreach (InstalledVoice voice in synth.GetInstalledVoices())
            {
                VoiceInfo info = voice.VoiceInfo;
                comboBox1.Items.Add(voice.VoiceInfo.Name);
            }
        }

        private void Synth_VoiceChange(object sender, VoiceChangeEventArgs e)
        {
            propertyGrid1.SelectedObject = synth.Voice;
            WriteLine("\r\nVoice changed!");
        }

        private void button2_Click(object sender, EventArgs e) => synth.SpeakAsync(textBox1.Text);

        private void button3_Click(object sender, EventArgs e) => synth.SpeakAsyncCancelAll();

        private void button4_Click(object sender, EventArgs e)
        {
            switch (synth.State)
            {
                case SynthesizerState.Ready:
                    synth.SpeakAsync(textBox1.Text);
                    break;
                case SynthesizerState.Speaking:
                    synth.Pause();
                    break;
                case SynthesizerState.Paused:
                    synth.Resume();
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                synth.SelectVoice(comboBox1.Text);
            }
            catch
            {
                comboBox1.Text = synth.Voice.Name;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            synth.SetOutputToWaveFile(saveFileDialog1.FileName);
        }

        private void button6_Click(object sender, EventArgs e) => voiceHints.Setup(synth);

        private class VoiceHints
        {
            public VoiceGender Gender { get; set; }
            public VoiceAge Age { get; set; }
            public void Setup(SpeechSynthesizer synth) =>
            synth.SelectVoiceByHints(Gender, Age);
        }
    }
}
