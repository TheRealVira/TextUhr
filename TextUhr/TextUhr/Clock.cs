using System;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace TextUhr
{
    internal static class Clock
    {
        private static bool Completed;
        private static string ToRet;

        public static async Task<string> GetTime(DateTime timeToDisplay, bool speechRecognitionApproach = false)
        {
            var toRead = timeToDisplay.ToShortTimeString();
            if (!speechRecognitionApproach)
                return await Task<string>.Factory.StartNew(() =>
                    int.Parse(toRead.Split(':')[0]).NumberToWords() + " Uhr " +
                    int.Parse(toRead.Split(':')[1]).NumberToWords() +
                    " Minuten");
            toRead = toRead.Split(':')[0] + " Uhr " + toRead.Split(':')[1];
            return await Task<string>.Factory.StartNew(() =>
            {
                Test(toRead);
                return ToRet;
            });

        }

        public static void Say(string timeToSay)
        {
            using (var synth = new SpeechSynthesizer())
            {
                var culture = new PromptBuilder(new CultureInfo("de-DE"));
                culture.AppendText(timeToSay);
                synth.Speak(timeToSay);
            }
        }

        private static void Test(string time)
        {
            if (File.Exists("temp.wav")) File.Delete("temp.wav");

            using (var synthesizer = new SpeechSynthesizer())
            {
                var culture = new PromptBuilder(new CultureInfo("de-DE"));
                culture.AppendText(time);
                synthesizer.SetOutputToWaveFile("temp.wav");
                synthesizer.Speak(culture);
            }

            using (var understander = new SpeechRecognitionEngine(new CultureInfo("de-DE")))
            {
                understander.RecognizeCompleted += Understander_RecognizeCompleted;
                understander.SpeechRecognized += Understander_SpeechRecognized;
                var grammar = new DictationGrammar {Name = "Dictation Grammar"};
                understander.LoadGrammar(grammar);
                Completed = false;
                ToRet = string.Empty;
                understander.SetInputToWaveFile("temp.wav");

                understander.RecognizeAsync();

                while (!Completed)
                {
                }
            }
        }

        private static void Understander_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result != null) ToRet += e.Result.Text;
        }

        private static void Understander_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Completed = true;
        }

        public static string NumberToWords(this int number)
        {
            if (number == 0)
                return "Null";

            var words = "";
            if (number <= 0) return words;
            if (words != "")
                words += "und ";

            var unitsMap = new[]
            {
                "Null", "Eins", "Zwei", "Drei", "Vier", "Fünf", "Sex", "Sieben", "Acht", "Neun", "Zehn", "Elf",
                "Zwölf", "Dreizehn", "Vierzehn", "Fünfzehn", "Sechzehn", "Siebzehn", "Achzehn", "Neunzehn"
            };
            var tensMap = new[] {"Null", "Zehn", "Zwanzig", "Dreißig", "Vierzig", "Fünfzig"};

            if (number < 20)
            {
                words += unitsMap[number];
            }
            else
            {
                if (number % 10 > 0)
                    words += number % 10 == 1 ? "Ein-und-" : unitsMap[number % 10] + "-und-";
                words += tensMap[number / 10];
            }

            return words;
        }
    }
}