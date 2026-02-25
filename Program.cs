using System.Text;

var input = args.Length > 0 ? args[0] : "hello cobalt";
var bytes = Encoding.UTF8.GetBytes(input);

var hash = SimdHash.Hash32(bytes);
Console.WriteLine($"input='{input}' hash=0x{hash:X8}");