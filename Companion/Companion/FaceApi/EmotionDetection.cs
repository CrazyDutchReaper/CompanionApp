﻿using Companion.Enum;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Companion.FaceApi
{
    class EmotionDetection
    {
        public static async Task<List<Mood>> DetectFaceExtract(Bitmap bitmap)
        {
            // change image to stream
            var stream = ToStream(bitmap, ImageFormat.Png);
            IList<DetectedFace> detectedFaces;

            // Detect faces with all attributes from image
            // we don't need all these attributes, only emotion is sufficient for current iteration
            detectedFaces = await Instances.Authenticate().Face.DetectWithStreamAsync(stream, returnFaceAttributes: new List<FaceAttributeType> {
                FaceAttributeType.Accessories, FaceAttributeType.Age,
                FaceAttributeType.Blur, FaceAttributeType.Emotion, 
                FaceAttributeType.Exposure, FaceAttributeType.FacialHair,
                FaceAttributeType.Gender, FaceAttributeType.Glasses, 
                FaceAttributeType.Hair, FaceAttributeType.HeadPose,
                FaceAttributeType.Makeup, FaceAttributeType.Noise, 
                FaceAttributeType.Occlusion, FaceAttributeType.Smile });
            
            // debug
            OutputData(detectedFaces);

            // get the most likely emotion for each detected face
            var emotions = new List<Mood>();
            foreach (var face in detectedFaces)
            {
                var list = new List<double>() {
                                   face.FaceAttributes.Emotion.Anger,
                                   face.FaceAttributes.Emotion.Fear,
                                   face.FaceAttributes.Emotion.Happiness,
                                   face.FaceAttributes.Emotion.Neutral,
                                   face.FaceAttributes.Emotion.Sadness,
                                   face.FaceAttributes.Emotion.Surprise
                };
                var mood = GetMood(list);
                emotions.Add(mood);
            }
            stream.Dispose();
            return emotions;
        }

        // gets mood through index casting
        private static Mood GetMood(List<double> inputs)
        {
            var highest = inputs.Max();
            var index = inputs.IndexOf(highest);
            return (Mood)index;
        }

        // write all the values we get, for debug purposes
        private static void OutputData(IList<DetectedFace> detectedFaces)
        {
            Console.WriteLine($"{detectedFaces.Count} face(s) detected from image");
            foreach (var face in detectedFaces)
            {
                Console.WriteLine("Anger " + face.FaceAttributes.Emotion.Anger);
                Console.WriteLine("Contempt " + face.FaceAttributes.Emotion.Contempt);
                Console.WriteLine("Disgust " + face.FaceAttributes.Emotion.Disgust);
                Console.WriteLine("Fear " + face.FaceAttributes.Emotion.Fear);
                Console.WriteLine("Happiness " + face.FaceAttributes.Emotion.Happiness);
                Console.WriteLine("Neutral " + face.FaceAttributes.Emotion.Neutral);
                Console.WriteLine("Sadness " + face.FaceAttributes.Emotion.Sadness);
                Console.WriteLine("Surprise " + face.FaceAttributes.Emotion.Surprise);
            }
        }

        // change image to stream
        public static Stream ToStream(Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}