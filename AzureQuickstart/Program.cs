using static System.Console;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

class Program {
    // Add your Computer Vision subscription key and endpoint
    static readonly string subscriptionKey = "YOUR-SECRET-COMES-HERE";
    static readonly string endpoint = "YOUR-SECRET-COMES-HERE";

    // Download these images (link in prerequisites), or you can use any appropriate image on your local machine.
    private const string READ_TEXT_LOCAL_IMAGE = "test-koncept.jpg";
    private const string READ_TEXT_URL_IMAGE = "https://intelligentkioskstore.blob.core.windows.net/visionapi/suggestedphotos/3.png";

    static void Main() {
        WriteLine("Azure Cognitive Services Computer Vision - .NET quickstart example\n");

        ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

        // Extract text (OCR) from a local image using the Read API
        ReadFileLocal(client, READ_TEXT_LOCAL_IMAGE).Wait();

        WriteLine("----------------------------------------------------------\n");
        WriteLine("Computer Vision quickstart is complete.\n");
    }

	#region Authenticate
	public static ComputerVisionClient Authenticate(string endpoint, string key) {
        ComputerVisionClient client = new (new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        return client;
    }
	#endregion

	#region ReadLocalFile
	public static async Task ReadFileLocal(ComputerVisionClient client, string localFile) {
        WriteLine("----------------------------------------------------------");
        WriteLine("READ FILE FROM LOCAL");
        WriteLine();

        // Read text from URL
        var textHeaders = await client.ReadInStreamAsync(File.OpenRead(localFile));
        // After the request, get the operation location (operation ID)
        string operationLocation = textHeaders.OperationLocation;
        Thread.Sleep(2000);
 
        WriteLine("");

        // Retrieve the URI where the recognized text will be stored from the Operation-Location header.
        // We only need the ID and not the full URL
        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        WriteLine("Operation ID:");
        WriteLine(operationId);

        // Extract the text
        ReadOperationResult results;
        WriteLine($"Reading text from local file {Path.GetFileName(localFile)}...");
        WriteLine();
        do {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }
        while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));

        WriteLine();
        var textUrlFileResults = results.AnalyzeResult.ReadResults;
        foreach (ReadResult page in textUrlFileResults) {
            foreach (Line line in page.Lines) {
                WriteLine(line.Text);
            }
        }
        WriteLine();
    }
    #endregion
}

/*
 * Computer Vision SDK QuickStart
 *
 * Examples included:
 *  - Authenticate
 *  - OCR (Read API): Read file from URL
 #  - OCR (Read API): Read file from local
 *
 *  Prerequisites:
 *   - Visual Studio 2019 (or 2017, but note this is a .Net Core console app, not .Net Framework)
 *   - NuGet library: Microsoft.Azure.CognitiveServices.Vision.ComputerVision
 *   - Azure Computer Vision resource from https://ms.portal.azure.com
 *   - Create a .Net Core console app, then copy/paste this Program.cs file into it. Be sure to update the namespace if it's different.
 *   - Download local images (celebrities.jpg, objects.jpg, handwritten_text.jpg, and printed_text.jpg)
 *     from the link below then add to your bin/Debug/netcoreapp2.2 folder.
 *     https://github.com/Azure-Samples/cognitive-services-sample-data-files/tree/master/ComputerVision/Images
 *
 *   How to run:
 *    - Once your prerequisites are complete, press the Start button in Visual Studio.
 *    - Each example displays a printout of its results.
 *
 *   References:
 *    - .NET SDK: https://docs.microsoft.com/en-us/dotnet/api/overview/azure/cognitiveservices/client/computervision?view=azure-dotnet
 *    - API (testing console): https://westus.dev.cognitive.microsoft.com/docs/services/computer-vision-v3-2/operations/5d986960601faab4bf452005
 *    - Computer Vision documentation: https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/
 */

/*
 *         // Extract text (OCR) from a URL image using the Read API
        // ReadFileUrl(client, READ_TEXT_URL_IMAGE).Wait();
        // </snippet_main_calls>
 * 
 * 
 // <snippet_readfileurl_1>
		#region ReadFromURL
		public static async Task ReadFileUrl(ComputerVisionClient client, string urlFile) {
            WriteLine("----------------------------------------------------------");
            WriteLine("READ FILE FROM URL");
            WriteLine();

            // Read text from URL
            var textHeaders = await client.ReadAsync(urlFile);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);
            // </snippet_readfileurl_1>

            // <snippet_readfileurl_2>
            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            Console.WriteLine($"Extracting text from URL file {Path.GetFileName(urlFile)}...");
            Console.WriteLine();
            do {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));
            // </snippet_readfileurl_2>

            // <snippet_readfileurl_3>
            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults) {
                foreach (Line line in page.Lines) {
                    Console.WriteLine(line.Text);
                }
            }
            Console.WriteLine();
        }
		// </snippet_readfileurl_3>
		#endregion
 
 */