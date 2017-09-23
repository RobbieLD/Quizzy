using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quizzy.Common
{
    public static class Extentions
    {
        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return task.IsCompleted
                ? task
                : task.ContinueWith(
                    completedTask => completedTask.GetAwaiter().GetResult(),
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        /// <summary>
        /// Encode messages with the first 4 bytes as the length of the stream
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public static void SendMessage(this TcpClient client, string message)
        {
            // Get the message bytes
            byte[] bytes = message.EncodeMessage();

            byte[] bytesLength = BitConverter.GetBytes(bytes.Length);

            NetworkStream stream = client.GetStream();

            stream.WriteAsync(bytesLength, 0, 4);
            stream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Encode messages with the first 4 bytes as the length of the stream
        /// </summary>
        /// <param name="client"></param>
        /// <param name="question"></param>
        public static void SendQuestion(this TcpClient client, Result question)
        {
            // Get the question bytes
            byte[] bytes = question.GetBytes();
            
            // now we have to encode length of the byte array into the first 4 bytes of the mesage
            byte[] bytesLength = BitConverter.GetBytes(bytes.Length);

            NetworkStream stream = client.GetStream();

            stream.WriteAsync(bytesLength, 0, 4);
            stream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// The first 4 bytes read in will always be a number indicating the length of the message
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bufferPrfixSize"></param>
        /// <returns></returns>
        public static async Task<object> ReadDataAsync(this TcpClient client, int bufferPrfixSize)
        {
            byte[] prefixBuffer = new byte[bufferPrfixSize];

            NetworkStream stream = client.GetStream();

            await stream.ReadAsync(prefixBuffer, 0, bufferPrfixSize);

            int messageLength = BitConverter.ToInt32(prefixBuffer, 0);

            byte[] buffer = new byte[messageLength];

            await stream.ReadAsync(buffer, 0, messageLength);

            // try and convert the stream into a question. If that fails just return it as a string
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    return (Result)formatter.Deserialize(memoryStream);
                }
            }
            catch
            {
                return buffer.DecodeMessage();
            }
        }

        public static string DecodeMessage(this byte[] bytes) => Encoding.ASCII.GetString(bytes);

        public static byte[] EncodeMessage(this string message) => Encoding.ASCII.GetBytes(message);

        public static byte[] GetBytes(this Result question)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, question);
                return stream.ToArray();
            }
        }
    }
}
