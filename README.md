# OpenSim-Configurable-Random-Wind

## ConfigurableRandomWind Plugin
The ConfigurableRandomWind plugin is designed to provide wind simulation in virtual environments. It allows you to configure and generate random winds with adjustable direction and strength.

## Class Information
- **Class Name:**ConfigurableRandomWind
- **Namespace:**OpenSim.Region.CoreModules.World.Wind.Plugins
- **Version:**1.0.0.0

## Plugin Features
1.*Random Wind Generation:**The plugin allows you to generate random winds with a customizable wind direction and strength.
2. **Configuration:**You can configure the wind parameters such as average strength, average direction, variance in strength, variance in direction, and the rate of change.
3. **Enable/Disable Random Wind:**The plugin can be configured to enable or disable random wind generation.

## Configuration
The following configuration parameters can be adjusted to control the behavior of the wind:
- strength: The average wind strength.
- avg_strength: An alternative to strength, the average wind strength.
- avg_direction: The average wind direction in degrees.
- var_strength: Allowable variance in wind strength.
- var_direction: Allowable variance in wind direction in degrees.
- rate_change: The rate of change in wind direction.
- random_wind_enabled: Enable random wind generation (true/false).

## Methods
void Initialise()
-This method initializes the wind plugin.
void Dispose()
- This method disposes of any resources used by the wind plugin.
void WindConfig(OpenSim.Region.Framework.Scenes.Scene scene, Nini.Config.IConfig windConfig)
- This method configures the wind plugin based on the provided configuration settings.
bool WindUpdate(uint frame)
- This method updates the wind simulation. If random wind generation is enabled, it generates random wind with adjustable parameters.
Vector3 WindSpeed(float fX, float fY, float fZ)
- This method returns the wind speed as a Vector3 at the given coordinates.
Vector2[] WindLLClientArray()
- This method returns an array of Vector2 containing wind speed information.
string Description
- This property provides a brief description of the plugin's functionality.
Dictionary<string, string> WindParams()
- This method returns a dictionary of wind parameters and their descriptions.
void WindParamSet(string param, float value)
- This method allows you to set specific wind parameters.
float WindParamGet(string param)
- This method retrieves the value of a specific wind parameter.

## Example Configuration
To enable random wind with adjustable parameters, you can add the following settings to your configuration file:

## Config OpenSim.ini
     [WindConfig]
     strength = 7.0
     avg_direction = 45.0
     var_strength = 2.0
     var_direction = 15.0
     rate_change = 1.5
     random_wind_enabled = true

# Deutsch

## Konfigurierbares RandomWind-Plugin
Das ConfigurableRandomWind-Plugin dient zur Windsimulation in virtuellen Umgebungen. Sie können damit zufällige Winde mit einstellbarer Richtung und Stärke konfigurieren und erzeugen.
Klasseninformationen
- **Klassenname:**ConfigurableRandomWind
- **Namespace:**OpenSim.Region.CoreModules.World.Wind.Plugins
- **Version:**1.0.0.0
## Plugin-Funktionen
1.*Zufällige Winderzeugung:**Mit dem Plugin können Sie zufällige Winde mit anpassbarer Windrichtung und -stärke erzeugen.
2. **Konfiguration:**Sie können die Windparameter wie durchschnittliche Stärke, durchschnittliche Richtung, Stärkevarianz, Richtungsvarianz und Änderungsrate konfigurieren.
3. **Zufälligen Wind aktivieren/deaktivieren:**Das Plugin kann so konfiguriert werden, dass die zufällige Winderzeugung aktiviert oder deaktiviert wird.
## Aufbau
Zur Steuerung des Windverhaltens können folgende Konfigurationsparameter angepasst werden:
- Stärke: Die durchschnittliche Windstärke.
- avg_strength: Eine Alternative zur Stärke, der durchschnittlichen Windstärke.
- avg_direction: Die durchschnittliche Windrichtung in Grad.
- var_strength: Zulässige Abweichung der Windstärke.
- var_direction: Zulässige Abweichung der Windrichtung in Grad.
- rate_change: Die Änderungsrate der Windrichtung.
- random_wind_enabled: Zufällige Winderzeugung aktivieren (wahr/falsch).
## Methoden
void Initialise()
-Diese Methode initialisiert das Wind-Plugin.
void Dispose()
– Diese Methode entsorgt alle vom Wind-Plugin verwendeten Ressourcen.
void WindConfig(OpenSim.Region.Framework.Scenes.Scene Szene, Nini.Config.IConfig windConfig)
– Diese Methode konfiguriert das Wind-Plugin basierend auf den bereitgestellten Konfigurationseinstellungen.
bool WindUpdate(uint-Frame)
- Diese Methode aktualisiert die Windsimulation. Wenn die zufällige Winderzeugung aktiviert ist, wird zufälliger Wind mit einstellbaren Parametern erzeugt.
Vector3 WindSpeed(float fX, float fY, float fZ)
– Diese Methode gibt die Windgeschwindigkeit als Vector3 an den angegebenen Koordinaten zurück.
Vector2[] WindLLClientArray()
– Diese Methode gibt ein Array von Vector2 zurück, das Informationen zur Windgeschwindigkeit enthält.
Zeichenfolge Beschreibung
– Diese Eigenschaft bietet eine kurze Beschreibung der Funktionalität des Plugins.
Dictionary<string, string> WindParams()
– Diese Methode gibt ein Wörterbuch mit Windparametern und deren Beschreibungen zurück.
void WindParamSet(string param, float value)
- Mit dieser Methode können Sie bestimmte Windparameter einstellen.
float WindParamGet(string param)
– Diese Methode ruft den Wert eines bestimmten Windparameters ab.
## Beispielkonfiguration
Um zufälligen Wind mit einstellbaren Parametern zu ermöglichen, können Sie die folgenden Einstellungen zu Ihrer Konfigurationsdatei hinzufügen:

## OpenSim.ini konfigurieren
     [WindConfig]
     Stärke = 7,0
     avg_direction = 45,0
     var_strength = 2,0
     var_direction = 15.0
     rate_change = 1,5
     random_wind_enabled = true
