using static System.Console;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace _02_OCR {
	static class Program {
		// DOC: https://docs.microsoft.com/cs-cz/azure/cognitive-services/computer-vision/quickstarts/csharp-print-text

		// Replace <Subscription Key> with your valid subscription key.
		const string subscriptionKey = "YOUR-SECRET";

		// You must use the same Azure region in your REST API method as you used to
		// get your subscription keys. For example, if you got your subscription keys
		// from the West US region, replace "westcentralus" in the URL
		// below with "westus".
		//
		// Free trial subscription keys are generated in the "westus" region.
		// If you use a free trial subscription key, you shouldn't need to change
		// this region.
		const string uriBase = "https://westeurope.api.cognitive.microsoft.com/vision/v3.2/ocr";

		static void Main() {
			// Get the path and filename to process from the user.
			WriteLine("Optical Character Recognition:");
			Write("Enter the path to an image with text you wish to read: ");
			string imageFilePath = ReadLine();

			if (File.Exists(imageFilePath)) {
				// Call the REST API method.
				WriteLine("\nWait a moment for the results to appear.\n");
				MakeOCRRequest(imageFilePath).Wait();
			} else {
				WriteLine("\nInvalid file path");
			}
			WriteLine("\nPress Enter to exit...");
			ReadLine();
		}

		/// <summary>
		/// Gets the text visible in the specified image file by using
		/// the Computer Vision REST API.
		/// </summary>
		/// <param name="imageFilePath">The image file with printed text.</param>
		static async Task MakeOCRRequest(string imageFilePath) {
			try {
				HttpClient client = new HttpClient();

				// Request headers.
				client.DefaultRequestHeaders.Add(
					"Ocp-Apim-Subscription-Key", subscriptionKey);

				// Request parameters. 
				// The language parameter doesn't specify a language, so the 
				// method detects it automatically.
				// The detectOrientation parameter is set to true, so the method detects and
				// and corrects text orientation before detecting text.
				string requestParameters = "language=unk&detectOrientation=true";

				// Assemble the URI for the REST API method.
				string uri = uriBase + "?" + requestParameters;

				HttpResponseMessage response;

				// Read the contents of the specified local image
				// into a byte array.
				byte[] byteData = GetImageAsByteArray(imageFilePath);

				// Add the byte array as an octet stream to the request body.
				using (ByteArrayContent content = new ByteArrayContent(byteData)) {
					// This example uses the "application/octet-stream" content type.
					// The other content types you can use are "application/json"
					// and "multipart/form-data".
					content.Headers.ContentType =
						new MediaTypeHeaderValue("application/octet-stream");

					// Asynchronously call the REST API method.
					response = await client.PostAsync(uri, content);
				}

				// Asynchronously get the JSON response.
				string contentString = await response.Content.ReadAsStringAsync();

				// Display the JSON response.
				string responseToString = $"\nResponse:\n\n{JsonPrettify(contentString)}\n";
				WriteLine(responseToString);

				using StreamWriter sw = new("OCROutput.json");
				sw.WriteLine(responseToString);

			} catch (Exception e) {
				WriteLine("\n" + e.Message);
			}
		}

		/// <summary>
		/// Returns the contents of the specified file as a byte array.
		/// </summary>
		/// <param name="imageFilePath">The image file to read.</param>
		/// <returns>The byte array of the image data.</returns>
		static byte[] GetImageAsByteArray(string imageFilePath) {
			// Open a read-only file stream for the specified file.
			using (FileStream fileStream =
				new FileStream(imageFilePath, FileMode.Open, FileAccess.Read)) {
				// Read the file's contents into a byte array.
				BinaryReader binaryReader = new BinaryReader(fileStream);
				return binaryReader.ReadBytes((int)fileStream.Length);
			}
		}

		public static string JsonPrettify(string json) {
			using (var stringReader = new StringReader(json))
			using (var stringWriter = new StringWriter()) {
				var jsonReader = new JsonTextReader(stringReader);
				var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
				jsonWriter.WriteToken(jsonReader);
				return stringWriter.ToString();
			}
		}
	}
}