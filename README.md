# UnityFontGlyphWriter
Unity component allows you to write glyphs from font asset to .png files

# Get Started
1. Attach FontGlyphWriter component to any game object on the scene.
2. Select font asset. And drop font asset to the "Font" field of the FontGlyphWriter component on your scene.
3. If your font asset is dynamic write charset of glyphs to the FontGlyphWriter's "Charset" field.
   Otherwise, if you won't make font dynamic for the moment by any reason, you can select "Character" field of the font asset,  select "Charset" and write your glyphs you want to export to the appeared on the bottom "Charset" field. I recommend you to use first approach.
4. Then I recommend you to change "Font Size" field value to 100 or somethimg like that. Otherwise your glyphs will have a trash resolution. (Dont't forget to change it back, after exporting)
5. Select context menu (gear icon) of FontGlyphWriter component in your scene and select "Write glyphs"
6. Pick a folder to export
7. You're awesome!

If you have any problem or question you can write me an email: abu.amir@mail.ru or instagram @_akinato.
