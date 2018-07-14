// Вставьте сюда финальное содержимое файла GhostsTask.cs
using System;
using System.Runtime.CompilerServices;
using System.Text;
 
namespace hashes
{
    public class GhostsTask :
        IFactory<Document>, IFactory<Vector>, IFactory<Segment>, IFactory<Cat>, IFactory<Robot>,
        IMagic
    {
        private byte[] documentArray = { 1, 1, 2 };
        Document document;
        Segment segment;
        Cat cat = new Cat("Tikhon", "Attic basement", DateTime.MaxValue);
        Vector vector = new Vector(0, 0);
        Robot robot = new Robot("C-3PO");
 
        public GhostsTask()
        {
            segment = new Segment(vector, vector);
            document = new Document("Russian Hackers", Encoding.Unicode, documentArray);
        }
 
        public void DoMagic()
        {
            documentArray[0] = 10;
            vector.Add(new Vector(2, 28));
            cat.Rename("Tisha");
            Robot.BatteryCapacity++;
        }
  
        Document IFactory<Document>.Create()
        {
            return document;
        }
 
        Vector IFactory<Vector>.Create()
        {
            return vector;
        }

        Segment IFactory<Segment>.Create()
        {
            return segment;
        }
 
        Cat IFactory<Cat>.Create()
        {
            return cat;
        }
 
        Robot IFactory<Robot>.Create()
        {
            return robot;
        }
    }
}