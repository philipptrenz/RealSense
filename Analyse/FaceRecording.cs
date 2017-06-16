using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RealSense
{
    [Serializable]
    public class FaceRecording
    {
        private String name;
        private PXCMFaceData.LandmarkPoint[][] data;
        private PXCMFaceData.LandmarkPoint[] nullData;
        float[,,] nullFaceVectors;

        public FaceRecording(String emotionName)
        {
            name = DateTime.Now.ToString("dd_MM_HH_mm_ss") + "." + emotionName;
        }

        public void setData(PXCMFaceData.LandmarkPoint[][] d, PXCMFaceData.LandmarkPoint[] nd)
        {
            data = d;
            nullData = nd;
            nullFaceVectors = calcLandmarkDiffs(59, 16, nullData);
        }

        public void setNullData(PXCMFaceData.LandmarkPoint[] nd)
        {
            nullData = nd;
            nullFaceVectors = calcLandmarkDiffs(59, 16, nullData);
        }

        public void save()
        {
            string serializationFile = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Recordings", name);
            Stream stream = File.Open(serializationFile, FileMode.Create);
            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, this);
        }

        public static FaceRecording load(String n)
        {
            Stream stream = File.Open(n, FileMode.Open);
            return (FaceRecording)new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(stream);
        }

        public void exportCSV()
        {
            int pOIn = 59, skip = 16;

            string csvFile = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Recordings", name + ".csv");

            float[][,,] currentFaceVectorFrames = new float[data.Length][,,];   // I was here, greets from philipp

            for (int frame = 0; frame < data.Length; frame++)
            {
                currentFaceVectorFrames[frame] = calcLandmarkDiffs(pOIn, skip, data[frame]);
            }

            float[][,,] allDiffs = new float[data.Length][,,];

            for (int i = 0; i < data.Length; i++)
            {
                float[,,] currentFaceVectors = currentFaceVectorFrames[i];
                allDiffs[i] = calcVectorDiffs(pOIn, currentFaceVectors, nullFaceVectors);
            }

            // parse to MASSIVE csv file
            foreach (float[,,] differencePerFrame in allDiffs)                           // each frame
            {
                string output = "";
                for (int firstIndex = 0; firstIndex < pOIn; firstIndex++)                // each landmark
                {
                    for (int secondIndex = 0; secondIndex < pOIn; secondIndex++)        // each landmark ... again
                    {
                        output += differencePerFrame[firstIndex, secondIndex, 0] + "," + differencePerFrame[firstIndex, secondIndex, 1] + "," + differencePerFrame[firstIndex, secondIndex, 2] + ";";
                    }
                }
                File.AppendAllText(csvFile, output + Environment.NewLine);
            }
            Console.WriteLine("Writing csv file finished");
        }

        public float[,,] calcVectorDiffs(int pOIn, float[,,] vecs, float[,,] nullFace)
        {
            for (int vectorRowIndex = 0; vectorRowIndex < pOIn; vectorRowIndex++)
            {
                for (int vectorColIndex = 0; vectorColIndex < pOIn; vectorColIndex++)
                {
                   // Console.WriteLine("@" + vectorRowIndex + "x" + vectorColIndex);
                    float xDiff = vecs[vectorRowIndex, vectorColIndex, 0] - nullFace[vectorRowIndex, vectorColIndex, 0];
                    float yDiff = vecs[vectorRowIndex, vectorColIndex, 1] - nullFace[vectorRowIndex, vectorColIndex, 1];
                    float zDiff = vecs[vectorRowIndex, vectorColIndex, 2] - nullFace[vectorRowIndex, vectorColIndex, 2];
                    vecs[vectorRowIndex, vectorColIndex, 0] = xDiff;
                    vecs[vectorRowIndex, vectorColIndex, 1] = yDiff;
                    vecs[vectorRowIndex, vectorColIndex, 2] = zDiff;
                }
            }

            return vecs;
        }

        public float[,,] calcLandmarkDiffs(int pOIn, int skip, PXCMFaceData.LandmarkPoint[] d)
        {
            float[,,] vectors = new float[pOIn, pOIn, 3];
            for (int firstIndex = 0; firstIndex < pOIn; firstIndex++)
            {
                int firstLandmarkID = firstIndex < 53 ? firstIndex : firstIndex + skip;
                PXCMFaceData.LandmarkPoint firstPoint = d[firstLandmarkID];

                for (int secondIndex = 0; secondIndex < pOIn; secondIndex++)
                {
                    int secondLandmarkID = secondIndex < 53 ? secondIndex : secondIndex + skip;
                    PXCMFaceData.LandmarkPoint secondPoint = d[secondLandmarkID];

                    float xDiff = firstPoint.world.x - secondPoint.world.x;
                    float yDiff = firstPoint.world.y - secondPoint.world.y;
                    float zDiff = firstPoint.world.z - secondPoint.world.z;

                    vectors[firstIndex, secondIndex, 0] = xDiff;
                    vectors[firstIndex, secondIndex, 1] = yDiff;
                    vectors[firstIndex, secondIndex, 2] = zDiff;
                }
            }

            return vectors;
        }

        public string currentFaceCSV(PXCMFaceData.LandmarkPoint[] currentF)
        {
            float[,,] currentLandmarkDiffs = calcLandmarkDiffs(59, 16, currentF);
            Console.WriteLine((currentF == null) + " - " + (nullFaceVectors == null));
            Console.WriteLine("DErp");
            float[,,] normalizedData = calcVectorDiffs(59, nullFaceVectors, currentLandmarkDiffs);
            string output = "";
            for (int firstIndex = 0; firstIndex < 59; firstIndex++)                // each landmark
            {
                for (int secondIndex = 0; secondIndex < 59; secondIndex++)        // each landmark ... again
                {
                    output += normalizedData[firstIndex, secondIndex, 0] + "," + normalizedData[firstIndex, secondIndex, 1] + "," + normalizedData[firstIndex, secondIndex, 2] + ";";
                }
            }
            return output;
        }

        public PXCMFaceData.LandmarkPoint[] getFace(int frame)
        {
            return data[frame];
        }

        public PXCMFaceData.LandmarkPoint[] getNullFace()
        {
            return nullData;
        }

    }
}
