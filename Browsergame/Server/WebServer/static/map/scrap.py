import urllib.request
import os 

def download(x,y,z):
    x = str(x)
    y = str(y)
    z = str(z)
    directory = "map/"+x+"/"+y+"/"
    img = z+".png"
    if os.path.isfile(directory+img): return
    if not os.path.exists(directory): os.makedirs(directory)

    urllib.request.urlretrieve("https://c.tile.openstreetmap.se/hydda/base/"+x+"/"+y+"/"+z+".png", directory+img)
    print("Downloaded "+directory+img)

def downloadAll(zoom, centerX, centerY, radius):
    for x in range(centerX-radius,centerX+radius): # Left/Right 247,350
        for y in range(centerY-radius, centerY+radius): # Left/Right 170,200
            download(zoom, x, y)

radius = 20
downloadAll(12, 2100, 1420, radius)
downloadAll(11, 1060, 715, radius)
downloadAll(10, 525, 355, radius)
downloadAll(9, 300, 180, radius)


