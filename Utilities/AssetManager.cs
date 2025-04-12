using SpaceShooter.Globals;

namespace SpaceShooter.Utilities
{
    internal class AssetManager
    {
        // Base path for assets
        public static string Path = "..\\..\\..\\Resources\\";

        // List of background images in the "background" folder
        public static List<string> background_images = [.. Directory.GetFiles($"{Path}Background", "*.png")];

        // Avatar image and background image selection logic
        public static Image
            AvatarImage = Image.FromFile($"{Path}ProfilePictures\\avatar1.png"),
            backgroundImage = AppGlobals.CurrentUser != null ? Image.FromFile($"{Path}Background/{AppGlobals.CurrentUser.BackgroundImage}.png") : Image.FromFile($"{background_images[0]}");

        // Method to combine images based on text
        public static Image CombineImages(string text)
        {
            string path = $"{Path}Font\\Numbers";
            List<Image> images = []; // Corrected: Initialize an empty list

            // Iterate over each character in the input text
            foreach (char c in text)
            {
                // Determine image name based on character (assuming uppercase or original if not found)
                string imageName = char.IsLetter(c) ? char.ToUpper(c).ToString() : c.ToString();
                string imagePath = $"{path}\\{imageName}.png";

                // Check if the image file exists
                if (File.Exists(imagePath)) images.Add(Image.FromFile(imagePath));
                else images.Add(Image.FromFile($"{path}\\SPACE.png"));
            }

            // Calculate the dimensions of the combined image
            int totalWidth = images.Sum(img => img.Width);
            int maxHeight = images.Max(img => img.Height);

            // Create a new bitmap for the combined image
            Bitmap combinedImage = new(totalWidth, maxHeight);

            // Draw each image onto the combined bitmap
            using (Graphics g = Graphics.FromImage(combinedImage))
            {
                int currentX = 0;
                foreach (Image img in images)
                {
                    g.DrawImage(img, new Rectangle(currentX, 0, img.Width, img.Height));
                    currentX += img.Width; // Move to the next position
                }
            }

            return combinedImage;
        }
    }
}
