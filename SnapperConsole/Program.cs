// See https://aka.ms/new-console-template for more information
using SnapperCore;

Console.WriteLine("Hello, World!");


var imageService = new ImageService();

await imageService.SaveImage("https://www.game.co.uk/en/playstation-portal-2924759");