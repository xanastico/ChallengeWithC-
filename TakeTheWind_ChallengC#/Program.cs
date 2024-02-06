using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Newtonsoft.Json;

// Interfaces
/*public interface IDataHandler
{
    List<Vehicle> ReadData(string filePath);
    void WriteData(List<Vehicle> vehicles, string filePath);
}*/

public class Vehicle
{
    public string Type { get; set; }
    public string MotorType { get; set; }
    public int Power { get; set; }
    public List<Wheel> Wheels { get; set; }
    public string UniqueId { get; set; } // Objetivo 1 -ID's
    public string Color { get; set; } // Objetive 4 - Mudar a cor dos hibridos para vermelho
}

public class Wheel
{
    public int Size { get; set; }
    public int Pressure { get; set; }
}

public class Program
{
    static void Main()
    {
        int choice = 0;

        do
        {
            Console.WriteLine("=========== MENU ===========");
            Console.WriteLine("1. Converter para JSON");
            Console.WriteLine("2. Converter para XML");
            Console.WriteLine("3. Sair");
            Console.Write("Escolha uma opção: ");
            
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Escolha inválida. Por favor, escolha novamente.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    ConvertToJson();
                    break;
                case 2:
                    ConvertToXml();
                    break;
                case 3:
                    Console.WriteLine("Adeus!");
                    break;
                default:
                    Console.WriteLine("Opção inválida. Por favor, escolha novamente.");
                    break;
            }

        } while (choice != 3);
    }

    static void ConvertToJson()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("Challenge.xml");

        XmlNodeList vehiclesXml = doc.GetElementsByTagName("vehicle");
        List<Vehicle> vehicles = new List<Vehicle>();

        int uniqueIdCounter = 1;

        foreach (XmlNode vehicleXml in vehiclesXml)
        {
            Vehicle newVehicle = new Vehicle();

            newVehicle.Type = vehicleXml.Attributes["type"].Value;
            newVehicle.MotorType = vehicleXml.SelectSingleNode("motor")?.Attributes["type"].Value ?? "";
            newVehicle.Power = Convert.ToInt32(vehicleXml.SelectSingleNode("motor/power")?.InnerText ?? "0");
            newVehicle.Wheels = new List<Wheel>();

            XmlNodeList wheelsXml = vehicleXml.SelectNodes("wheels/wheel");
            foreach (XmlNode wheelXml in wheelsXml)
            {
                Wheel newWheel = new Wheel();
                newWheel.Size = Convert.ToInt32(wheelXml.Attributes["size"].Value);
                newWheel.Pressure = Convert.ToInt32(wheelXml.Attributes["pressure"].Value);
                newVehicle.Wheels.Add(newWheel);
            }

            newVehicle.UniqueId = "ID-" + uniqueIdCounter++; //Desafio 1
            vehicles.Add(newVehicle);
        }

        foreach (Vehicle vehicle in vehicles)
        {
            if (vehicle.Type == "truck")
            {
                vehicle.Power = 3000; //Desafio 2
            }
            else if (vehicle.Type == "bicycle" && vehicle.Wheels.Exists(w => w.Size == 14))
            {
                foreach (Wheel wheel in vehicle.Wheels)
                {
                    if (wheel.Size == 14)
                    {
                        wheel.Pressure = 50; //Desafio 3
                    }
                }
            }
            else if (vehicle.Type == "car" && vehicle.MotorType == "hybrid")
            {
                vehicle.Color = "red"; //Desafio 4
            }
        }

        string json = JsonConvert.SerializeObject(vehicles, Newtonsoft.Json.Formatting.Indented);

        File.WriteAllText("ChallengeOutput.json", json);
        Console.WriteLine("=======================================");
        Console.WriteLine("Atualizado e Guardado em Challenge.json");
        Console.WriteLine("=======================================");
        Console.WriteLine("");
    }

    static void ConvertToXml()
    {
        string jsonFromFile = File.ReadAllText("ChallengeOutput.json");
        List<Vehicle> vehiclesFromJson = JsonConvert.DeserializeObject<List<Vehicle>>(jsonFromFile);

        XmlDocument xmlDocument = new XmlDocument();
        XmlElement root = xmlDocument.CreateElement("vehicles");
        xmlDocument.AppendChild(root);

        foreach (Vehicle vehicle in vehiclesFromJson)
        {
            XmlElement vehicleElement = xmlDocument.CreateElement("vehicle");
            vehicleElement.SetAttribute("type", vehicle.Type);

            XmlElement motorElement = xmlDocument.CreateElement("motor");
            motorElement.SetAttribute("type", vehicle.MotorType);

            XmlElement powerElement = xmlDocument.CreateElement("power");
            powerElement.InnerText = vehicle.Power.ToString();

            motorElement.AppendChild(powerElement);
            vehicleElement.AppendChild(motorElement);

            XmlElement wheelsElement = xmlDocument.CreateElement("wheels");
            foreach (Wheel wheel in vehicle.Wheels)
            {
                XmlElement wheelElement = xmlDocument.CreateElement("wheel");
                wheelElement.SetAttribute("size", wheel.Size.ToString());
                wheelElement.SetAttribute("pressure", wheel.Pressure.ToString());
                wheelsElement.AppendChild(wheelElement);
            }
            vehicleElement.AppendChild(wheelsElement);

            root.AppendChild(vehicleElement);
        }

        xmlDocument.Save("ChallengeOutput.xml");
        Console.WriteLine("============================================");
        Console.WriteLine("Atualizado e Guardado em ChallengeOutput.xml");
        Console.WriteLine("============================================");
        Console.WriteLine("");
    }
}
