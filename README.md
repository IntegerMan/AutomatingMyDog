# AutomatingMyDog
Ridiculous reference materials demonstrating using Azure Cognitive Services to build intelligent applications using C# code

## Tasks

I want to get the following tasks done for demos:

Global App:
- [x] Set the basic layout of the desktop app
- [x] Global Navigation
- [x] Global error handling in the desktop app
- [x] Add a place in the desktop app to enter your cognitive services keys / endpoints
- [x] Save Settings to a Settings File
- [x] Extract a control for the chat region

Text / Chat:
- [x] Add a "listen" button to the desktop app
- [x] Add a text chat feature to the desktop app
- [x] Add a log of things the app has said to you
- [x] Send message on to Text Analytics APIs to get sentiment analysis etc.
- [x] Send message on to LUIS to get app responses
- [x] Speak a message as a result of the intent

Images: 
- [x] Add a Telerik Webcam to the desktop app
- [x] Add a "take a picture" button to the desktop app
- [x] Save snapshot to snapshot.png
- [x] Include source image in image chat
- [x] Send image on to computer vision
- [x] Include image tags, etc. in chat results
- [x] Object bounding boxes in image chat
- [x] Smart thumbnail results in image chat

## Future Enhancements
For KCDC these enhancements would help "sell" it more:

### Onboarding Improvements
- [ ] Expand the `README.md` to include a description of the project and getting started configuration
- [ ] Expand the `README.md` to include links to helpful content I've made
- [ ] Investigate Telerik use by non-license holders
- [ ] Add downloadable files in GitHub Releases

### Error Handling
- [ ] No Internet Connection Handling
- [ ] Too Large File Handling
- [ ] Bad Key or Endpoint Handling
- [ ] Non-Image or locked file error handling on Dragon Drop
- [ ] Offline Mode

### Global Improvements
- [ ] Additional styling and polish
- [x] Chat on the sidebar at all times
- [ ] Better global navigation
- [ ] Add filter buttons to filter out irrelevant messages
- [ ] Link to Settings Page from Welcome Page
- [ ] Icons on Navigation and Buttons
- [ ] Include code examples in the app UI

### Hardware
- [ ] Mount a webcam on the stuffed dog
- [ ] Mount a speaker on the stuffed dog as a backup for HDMI audio-out issues
- [ ] Bring a stuffed animal to show DogOS

### Vision
- [x] Better card responses
- [x] Drop image onto chat from desktop to send the image
- [x] Respond with voice depending on what was detected in the image
- [ ] Add an "upload picture" button to the chat region
- [ ] Better Drop Preview for Drag / Drop Image
- [ ] Respond to Batman in image

### Speech
- [ ] Allow changing the voice of the dog to a set of presets
- [ ] Include a SSML example

### Language Understanding
- [ ] Add a greater amount of intents to the CLU model
- [ ] Move from LUIS to CLU
- [ ] Settings for LUIS / CLU
- [ ] Async / Await for Tasks
- [ ] Detect entities from LUIS / CLU

### Other Ideas
- [ ] Blazor Version
- [ ] Azure Bot Service Integration
- [ ] Azure OpenAI Integration
- [ ] Testimonial for Telerik