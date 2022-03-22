
using System;
using System.IO;
using System.Diagnostics;
using speechToTextMVC.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace speechToTextMVC.Controllers;

public class SpeechController : Controller
{
    private readonly ILogger<SpeechController> _logger;

    public SpeechController(ILogger<SpeechController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Audio(/* should take in an audio file */) 
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    static string YourSubscriptionKey = "1ebf10647ec64e5d84887b9331702759";
    static string YourServiceRegion = "eastus";
    public string OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                return speechRecognitionResult.Text;
            case ResultReason.NoMatch:
                return "CANCELED: Speech could not be recognized.";
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);

                if (cancellation.Reason == CancellationReason.Error)
                {
                    return $"CANCELED: ErrorCode={cancellation.ErrorCode}, ErrorDetails={cancellation.ErrorDetails}, Double check the speech resource key and region.";
                } else {
                    return $"CANCELED: {cancellation.Reason.ToString()}";
                }
            default: return "CANCELED: No speech assessed";
        }
    }
    public async Task<string> Listen(string filePath)
    {
        // set config vars
        var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);        
        speechConfig.SpeechRecognitionLanguage = "en-US";

        // load the audio file to be listened to by the speech recognizer
        using var audioConfig = AudioConfig.FromWavFileInput(filePath);
        using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        // convert speech to text
        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        return OutputSpeechRecognitionResult(speechRecognitionResult);
    }

}
