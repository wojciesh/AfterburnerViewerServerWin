using System.Diagnostics;

namespace AfterburnerViewerServerWin
{
    public class AfterburnerParser
    {
        public static List<MeasurementType> ReadMeasurementTypes(TextReader reader)
        {
            List<MeasurementType> measurementTypes = [];
            List<string> typeNames = [];
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("80,"))
                    break;

                if (line.StartsWith("02,"))
                {
                    typeNames = line.Split(',')
                        .Skip(2)
                        .Select(x => x.Trim())
                        .ToList();
                    continue;
                }

                if (!line.StartsWith("03,"))
                    continue;

                var types = ParseMeasurementType(line, typeNames);
                if (types != null)
                    measurementTypes.Add(types);
            }

            return measurementTypes;


            static MeasurementType? ParseMeasurementType(string typeInfoLine, List<string> typeNames)
            {
                Debug.Assert(typeInfoLine != null);

                try
                {
                    var parts = typeInfoLine.Split(',');

                    if (parts == null || parts.Length < 10)
                        throw new FormatException("Invalid type format in line: " + typeInfoLine);

                    var i = 2;
                    var index = int.Parse(parts[i].Trim());
                    i++;
                    var unit = parts[i].Trim();
                    i++;
                    var min = parts[i].Trim();
                    i++;
                    var max = parts[i].Trim();
                    i++;
                    var valBase = parts[i].Trim();
                    i += 2;
                    var format = parts[i].Trim();

                    return new MeasurementType
                    {
                        Name = typeNames[index],
                        Unit = unit,
                        Min = double.Parse(min),
                        Max = double.Parse(max),
                        Base = int.Parse(valBase),
                        Format = format
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<AfterburnerMeasurement> ExtractMeasurements(string measurementsLine, List<MeasurementType> measurementTypes)
        {
            if (measurementTypes == null)
                throw new ArgumentNullException(nameof(measurementTypes));

            if (string.IsNullOrWhiteSpace(measurementsLine))
                throw new ArgumentException($"'{nameof(measurementsLine)}' cannot be null or whitespace.", nameof(measurementsLine));

            if (!measurementsLine.StartsWith("80,"))
                throw new FormatException("Invalid measurements format (not starting with \"80\") in line: " + measurementsLine);

            var parts = measurementsLine.Split(',');
            
            if (parts == null || parts.Length < 10)
                throw new FormatException("Invalid measurements format in line: " + measurementsLine);

            return measurementTypes.Select((type, i) => new AfterburnerMeasurement 
            {
                Type = type,
                Value = double.Parse(parts[2 + i].Trim())
            }).ToList();
        }

        //static double Normalize(double val, double min, double max)
        //{
        //    return (val - min) / (max - min);
        //}
    }

    public record MeasurementType
    {
        public required string Name { get; init; }
        public required string Unit { get; init; }
        public required double Min { get; init; }
        public required double Max { get; init; }
        public required int Base { get; init; }
        public required string? Format { get; init; }
    }

    public record AfterburnerMeasurement
    {
        public required MeasurementType Type { get; init; }
        public required double Value { get; init; }
    }
}


/*
Input file:
Lines 00-01: Header
Line 02: Measurement Types
Lines starting with "03," are describing Measurements Types:
    after timestamp, the first number is the index of the Measurement Type (from Line 02), then Unit, Min, Max, "10" (probably the base, but not used in app), then 2 empty columns, and finally the Format (for floats)
Next are Linest starting with "80," which are the actual measurements:
    after timestamp, the first number is the value of the first Measurement Type (from Line 02), then the value of the next Measurement Type, and so on
```
00, 07-12-2024 23:49:46, Hardware monitoring log v1.6 
01, 07-12-2024 23:49:46, Radeon RX 560 Series
02, 07-12-2024 23:49:46, GPU temperature     ,GPU usage           ,FB usage            ,Memory usage        ,Core clock          ,Memory clock        ,Power               ,Fan speed           ,Fan tachometer      ,CPU1 temperature    ,CPU2 temperature    ,CPU3 temperature    ,CPU4 temperature    ,CPU5 temperature    ,CPU6 temperature    ,CPU7 temperature    ,CPU8 temperature    ,CPU9 temperature    ,CPU10 temperature   ,CPU11 temperature   ,CPU12 temperature   ,CPU temperature     ,CPU1 usage          ,CPU2 usage          ,CPU3 usage          ,CPU4 usage          ,CPU5 usage          ,CPU6 usage          ,CPU7 usage          ,CPU8 usage          ,CPU9 usage          ,CPU10 usage         ,CPU11 usage         ,CPU12 usage         ,CPU usage           ,CPU1 clock          ,CPU2 clock          ,CPU3 clock          ,CPU4 clock          ,CPU5 clock          ,CPU6 clock          ,CPU7 clock          ,CPU8 clock          ,CPU9 clock          ,CPU10 clock         ,CPU11 clock         ,CPU12 clock         ,CPU clock           ,CPU1 power          ,CPU2 power          ,CPU3 power          ,CPU4 power          ,CPU5 power          ,CPU6 power          ,CPU7 power          ,CPU8 power          ,CPU9 power          ,CPU10 power         ,CPU11 power         ,CPU12 power         ,CPU power           ,RAM usage           ,Commit charge       
03, 07-12-2024 23:49:46, 0                   ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 1                   ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 2                   ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 3                   ,MB                  ,0.000               ,8192.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 4                   ,MHz                 ,0.000               ,2500.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 5                   ,MHz                 ,0.000               ,7500.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 6                   ,W                   ,0.000               ,300.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 7                   ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 8                   ,RPM                 ,0.000               ,10000.000           ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 9                   ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 10                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 11                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 12                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 13                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 14                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 15                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 16                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 17                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 18                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 19                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 20                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 21                  ,°C                  ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 22                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 23                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 24                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 25                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 26                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 27                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 28                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 29                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 30                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 31                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 32                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 33                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 34                  ,%                   ,0.000               ,100.000             ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 35                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 36                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 37                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 38                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 39                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 40                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 41                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 42                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 43                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 44                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 45                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 46                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 47                  ,MHz                 ,0.000               ,5000.000            ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 48                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 49                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 50                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 51                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 52                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 53                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 54                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 55                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 56                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 57                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 58                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 59                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 60                  ,W                   ,0.000               ,200.000             ,10                  ,                    ,                    ,%.1f                
03, 07-12-2024 23:49:46, 61                  ,MB                  ,0.000               ,16384.000           ,10                  ,                    ,                    ,
03, 07-12-2024 23:49:46, 62                  ,MB                  ,0.000               ,16384.000           ,10                  ,                    ,                    ,
80, 07-12-2024 23:49:46, 46.000              ,0.000               ,0.000               ,829.055             ,214.000             ,1500.000            ,11.031              ,21.000              ,0.000               ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,67.500              ,6.250               ,48.438              ,14.063              ,0.000               ,3.125               ,26.563              ,6.250               ,0.000               ,0.000               ,0.000               ,4.688               ,1.563               ,9.245               ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,5.871               ,5.871               ,2.634               ,2.634               ,4.119               ,4.119               ,1.722               ,1.722               ,1.600               ,1.600               ,1.958               ,1.959               ,39.108              ,12008.000           ,21411.000           
80, 07-12-2024 23:49:47, 46.000              ,0.000               ,0.000               ,829.055             ,214.000             ,1500.000            ,11.039              ,21.000              ,0.000               ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,66.750              ,4.688               ,0.000               ,3.125               ,0.000               ,0.000               ,1.563               ,3.125               ,0.000               ,1.563               ,3.125               ,1.563               ,0.000               ,1.563               ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,1.631               ,1.631               ,1.312               ,1.312               ,1.676               ,1.676               ,1.197               ,1.197               ,0.792               ,0.792               ,0.954               ,0.953               ,30.867              ,12008.000           ,21416.000           
80, 07-12-2024 23:49:48, 46.000              ,0.000               ,0.000               ,829.055             ,214.000             ,1500.000            ,11.011              ,21.000              ,0.000               ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,66.000              ,4.688               ,1.563               ,1.563               ,1.563               ,0.000               ,1.563               ,0.000               ,1.563               ,0.000               ,0.000               ,1.563               ,0.000               ,1.172               ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,1.882               ,1.893               ,1.338               ,1.338               ,1.668               ,1.668               ,1.188               ,1.188               ,0.071               ,0.071               ,0.759               ,0.759               ,29.887              ,12007.000           ,21370.000           
80, 07-12-2024 23:49:49, 46.000              ,0.000               ,0.000               ,829.055             ,214.000             ,1500.000            ,11.011              ,21.000              ,0.000               ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,65.500              ,14.063              ,6.250               ,4.688               ,0.000               ,4.688               ,6.250               ,7.813               ,0.000               ,4.688               ,1.563               ,6.250               ,0.000               ,4.688               ,4300.000            ,4300.000            ,4300.000            ,4300.000            ,4300.000            ,4300.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,4300.000            ,2.315               ,2.304               ,1.653               ,1.653               ,2.005               ,2.005               ,1.490               ,1.490               ,0.892               ,0.892               ,1.227               ,1.227               ,32.095              ,11977.000           ,21368.000           
80, 07-12-2024 23:49:50, 46.000              ,1.000               ,0.000               ,850.180             ,214.000             ,1500.000            ,11.066              ,21.000              ,0.000               ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,65.000              ,9.375               ,1.563               ,3.125               ,1.563               ,4.688               ,4.688               ,0.000               ,0.000               ,4.688               ,0.000               ,1.563               ,1.563               ,2.734               ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3750.000            ,3775.000            ,3775.000            ,2.344               ,2.344               ,1.699               ,1.699               ,1.941               ,1.941               ,1.047               ,1.047               ,1.015               ,1.015               ,1.149               ,1.149               ,31.631              ,12002.000           ,21404.000           
80, 07-12-2024 23:49:51, 46.000              ,2.000               ,0.000               ,850.180             ,214.000             ,1500.000            ,11.035              ,21.000              ,0.000               ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,64.375              ,3.125               ,0.000               ,0.000               ,0.000               ,1.563               ,0.000               ,0.000               ,0.000               ,0.000               ,0.000               ,0.000               ,0.000               ,0.391               ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,3775.000            ,1.770               ,1.770               ,1.283               ,1.283               ,1.499               ,1.499               ,0.681               ,0.681               ,0.996               ,0.996               ,1.064               ,1.064               ,30.501              ,12003.000           ,21400.000           

```
*/
