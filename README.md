# ImageFunctions
Image manipulation utilities for Azure Functions


Uses cross-platform imaging library by [SixLabors](https://sixlabors.com/projects/imagesharp/). Provides simple image manipulation functionality in Azure Functions.


Usage:

http://.../ImageFlip?direction=[vertical | horizontal]

   Flips image in desired direction and returns as PNG.

http://.../ImageRotate?degrees=[0-360]

   Rotates image by specified degrees and returns as PNG.

http://.../ImageInfo

   Returns width and height of image as JSON.
