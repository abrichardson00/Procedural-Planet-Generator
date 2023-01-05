# Procedural Planet Generator
Experimenting with Unity, generating planet terrain and making the sky look cool with a raymarching shader.

![no atmosphere](planet1.jpg?raw=true "no atmosphere")
![clouds](planet2.jpg?raw=true "clouds")
![sunset](planet3.jpg?raw=true "sunset")

**Features:**
  * 3D procedurally generated hills and water using a combination of noise and simulating some simple rain / hydraulic erosion.
  * Planet sphere is divided into different 'square' mesh chunks (i.e. the planet is really a cube, inflated into a sphere shape). Only appropriate visable chunks are rendered depending on camera position. Further away visable chunks are also rendered in less detail.
  * I then later played around with volumetric raymarching shaders. I used some existing cloud rendering shader and adapted it into a more general 'sky' shader. This added blue / red colours depending on the sun position relative to the camera.

The rendering of different chunks at different details was a really fiddly but interesting problem. Rendering different levels of detail (LODs) are a standard feature in game engines for texture rendering etc - but I feel like it was a useful learning experience to implement this from scratch for my system. Also, when handling these different chunks on a sphere, it was fun having this direct use for linear algebra (i.e. handling position vectors and calculating vector projections). 

This isn't a downloadable whole Unity project - and even if it was, the project is 3 years old and probably won't compile. The last thing I was trying to do was simulate the orbits of multiple planet as well - which was a stupid idea considering how messy the code was becoming!
