require 'rmagick'

img = Magick::Image::read('solid.png')[0]

block_size = 64
edge_width = 4

edge_area = block_size * edge_width

(0...img.rows).step(block_size) do |y|
  (0...img.columns).step(block_size) do |x|
    pixel = img.get_pixels(x, y, 1, 1)[0]
    pixel_rgb = [ pixel.red, pixel.green, pixel.blue ]

    edge_pixels = []
    edge_rgb = pixel_rgb.map { |c| c * 0.9 }

    for i in 0...edge_area
      edge_pixels << Magick::Pixel.new(*edge_rgb)
    end

    # top
    img.store_pixels(x, y, block_size, edge_width, edge_pixels)

    # left
    img.store_pixels(x, y, edge_width, block_size, edge_pixels)

    # right
    img.store_pixels(x + block_size - edge_width, y, edge_width, block_size, edge_pixels)

    # bottom
    img.store_pixels(x, y + block_size - edge_width, block_size, edge_width, edge_pixels)

    bevel_length = block_size - (edge_width * 2)

    bevel_dark_pixels = []
    bevel_rgb_dark = pixel_rgb.map { |c| c * 0.85 }

    for i in 0...bevel_length
      bevel_dark_pixels << Magick::Pixel.new(*bevel_rgb_dark)
    end
    
    # bevel right
    img.store_pixels(x + block_size - edge_width - 1, y + edge_width, 1, bevel_length, bevel_dark_pixels)
    
    # bevel bottom
    img.store_pixels(x + edge_width, y + block_size - edge_width - 1, bevel_length, 1, bevel_dark_pixels)

    bevel_light_pixels = []
    bevel_rgb_light = pixel_rgb.map { |c| c * 1.05 }

    for i in 0...bevel_length
      bevel_light_pixels << Magick::Pixel.new(*bevel_rgb_light)
    end

    # bevel top
    img.store_pixels(x + edge_width, y + edge_width, bevel_length, 1, bevel_light_pixels)

    # bevel left
    img.store_pixels(x + edge_width, y + edge_width, 1, bevel_length, bevel_light_pixels)
  end
end

img.write("output.png")
