/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

 * ConfigurableRandomWind Plugin
 * The ConfigurableRandomWind plugin is designed to provide wind simulation in virtual environments. It allows you to configure and generate random winds with adjustable direction and strength.
 * Class Information
 * - **Class Name:**ConfigurableRandomWind
 * - **Namespace:**OpenSim.Region.CoreModules.World.Wind.Plugins
 * - **Version:**1.0.0.0
 * Plugin Features
 * 1. * *Random Wind Generation:**The plugin allows you to generate random winds with a customizable wind direction and strength.
 * 2. **Configuration:**You can configure the wind parameters such as average strength, average direction, variance in strength, variance in direction, and the rate of change.
 * 3. **Enable/Disable Random Wind:**The plugin can be configured to enable or disable random wind generation.
 * Configuration
 * The following configuration parameters can be adjusted to control the behavior of the wind:
 * - strength: The average wind strength.
 * - avg_strength: An alternative to strength, the average wind strength.
 * - avg_direction: The average wind direction in degrees.
 * - var_strength: Allowable variance in wind strength.
 * - var_direction: Allowable variance in wind direction in degrees.
 * - rate_change: The rate of change in wind direction.
 * - random_wind_enabled: Enable random wind generation (true/false).
 * Methods
 * void Initialise()
 * -This method initializes the wind plugin.
 * void Dispose()
 * - This method disposes of any resources used by the wind plugin.
 * void WindConfig(OpenSim.Region.Framework.Scenes.Scene scene, Nini.Config.IConfig windConfig)
 * - This method configures the wind plugin based on the provided configuration settings.
 * bool WindUpdate(uint frame)
 * - This method updates the wind simulation. If random wind generation is enabled, it generates random wind with adjustable parameters.
 * Vector3 WindSpeed(float fX, float fY, float fZ)
 * - This method returns the wind speed as a Vector3 at the given coordinates.
 * Vector2[] WindLLClientArray()
 * - This method returns an array of Vector2 containing wind speed information.
 * string Description
 * - This property provides a brief description of the plugin's functionality.
 * Dictionary<string, string> WindParams()
 * - This method returns a dictionary of wind parameters and their descriptions.
 * void WindParamSet(string param, float value)
 * - This method allows you to set specific wind parameters.
 * float WindParamGet(string param)
 * - This method retrieves the value of a specific wind parameter.
 * Example Configuration
 * To enable random wind with adjustable parameters, you can add the following settings to your configuration file:

 * OpenSim.ini
    [WindSettings]
    avg_strength = 5.0
    avg_direction = 0.0
    var_strength = 5.0
    var_direction = 30.0
    rate_change = 1.0
    random_wind_enabled = true
    north_wind_enabled = true
    northwest_wind_enabled = true
    northeast_wind_enabled = true
    south_wind_enabled = true
    southwest_wind_enabled = true
    southeast_wind_enabled = true
    west_wind_enabled = true
    east_wind_enabled = true
    angle_range = 10.0
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using OpenMetaverse;
using Mono.Addins;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.CoreModules.World.Wind;
using Nini.Config;

namespace OpenSim.Region.CoreModules.World.Wind.Plugins
{
    /// <summary>
    /// The `ConfigurableRandomWind` plugin is designed to provide wind simulation in virtual environments.
    /// It allows you to configure and generate random winds with adjustable direction and strength.
    /// </summary>
    [Extension(Path = "/OpenSim/WindModule", NodeName = "WindModel", Id = "ConfigurableRandomWind")]
    class ConfigurableRandomWind : Mono.Addins.TypeExtensionNode, IWindModelPlugin
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Vector2[] m_windSpeeds = new Vector2[16 * 16];

        private float m_avgStrength = 5.0f; // Average magnitude of the wind vector
        private float m_avgDirection = 0.0f; // Average direction of the wind in degrees
        private float m_varStrength = 5.0f; // Max Strength Variance
        private float m_varDirection = 30.0f; // Max Direction Variance
        private float m_rateChange = 1.0f;
        private bool m_randomWindEnabled = false;
        private bool m_northWindEnabled = true;
        private bool m_northwestWindEnabled = true;
        private bool m_northeastWindEnabled = true;
        private bool m_southWindEnabled = true;
        private bool m_southwestWindEnabled = true;
        private bool m_southeastWindEnabled = true;
        private bool m_eastWindEnabled = true;
        private bool m_westWindEnabled = true;
        private float m_angleRange = 10.0f; // ±10 degrees

        private Vector2 m_curPredominateWind = new Vector2();

        /// <summary>
        /// Gets the version of the wind plugin.
        /// </summary>
        public string Version
        {
            get { return "1.0.0.0"; }
        }

        /// <summary>
        /// Gets the name of the wind plugin.
        /// </summary>
        public string Name
        {
            get { return "ConfigurableRandomWind"; }
        }

        /// <summary>
        /// Initializes the wind plugin.
        /// </summary>
        public void Initialise()
        {
        }

        /// <summary>
        /// Disposes of any resources used by the wind plugin.
        /// </summary>
        public void Dispose()
        {
            m_windSpeeds = null;
        }

        /// <summary>
        /// Configures the wind plugin based on the provided configuration settings.
        /// </summary>
        /// <param name="scene">The scene where the wind plugin is active.</param>
        /// <param name="windConfig">The configuration settings for the wind.</param>
        public void WindConfig(OpenSim.Region.Framework.Scenes.Scene scene, Nini.Config.IConfig windConfig)
        {
            if (windConfig != null)
            {
                m_avgStrength = windConfig.GetFloat("strength", 5.0f);
                m_avgStrength = windConfig.GetFloat("avg_strength", 5.0f);
                m_avgDirection = windConfig.GetFloat("avg_direction", 0.0f);
                m_varStrength = windConfig.GetFloat("var_strength", 5.0f);
                m_varDirection = windConfig.GetFloat("var_direction", 30.0f);
                m_rateChange = windConfig.GetFloat("rate_change", 1.0f);
                m_randomWindEnabled = windConfig.GetBoolean("random_wind_enabled", false);
                m_northWindEnabled = windConfig.GetBoolean("north_wind_enabled", true);
                m_northwestWindEnabled = windConfig.GetBoolean("northwest_wind_enabled", true);
                m_northeastWindEnabled = windConfig.GetBoolean("northeast_wind_enabled", true);
                m_southWindEnabled = windConfig.GetBoolean("south_wind_enabled", true);
                m_southwestWindEnabled = windConfig.GetBoolean("southwest_wind_enabled", true);
                m_southeastWindEnabled = windConfig.GetBoolean("southeast_wind_enabled", true);
                m_eastWindEnabled = windConfig.GetBoolean("east_wind_enabled", true);
                m_westWindEnabled = windConfig.GetBoolean("west_wind_enabled", true);
                m_angleRange = windConfig.GetFloat("angle_range", 10.0f);
                LogSettings();
            }
        }

        /// <summary>
        /// Updates the wind simulation. If random wind generation is enabled, it generates random wind with adjustable parameters.
        /// </summary>
        /// <param name="frame">The frame number.</param>
        /// <returns>Returns true if the wind was updated successfully.</returns>
        public bool WindUpdate(uint frame)
        {
            if (m_randomWindEnabled)
            {
                double avgAng = m_avgDirection * (Math.PI / 180.0);
                double varDir = m_varDirection * (Math.PI / 180.0);

                Random random = new Random();
                double randomDir = avgAng + (random.NextDouble() * 2 - 1) * varDir;
                double randomSpeed = m_avgStrength + (random.NextDouble() * 2 - 1) * m_varStrength;

                if (randomSpeed < 0)
                    randomSpeed = -randomSpeed;

                m_curPredominateWind.X = (float)Math.Cos(randomDir);
                m_curPredominateWind.Y = (float)Math.Sin(randomDir);

                m_curPredominateWind.Normalize();
                m_curPredominateWind.X *= (float)randomSpeed;
                m_curPredominateWind.Y *= (float)randomSpeed;

                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        m_windSpeeds[y * 16 + x] = m_curPredominateWind;
                        m_windSpeeds[y * 16 + x] = CalculateWindSpeed(x, y, randomDir, randomSpeed);
                    }
                }
            }
            return true;
        }

        private Vector2 CalculateWindSpeed(int x, int y, double randomDir, double randomSpeed)
        {
            if (x == 8 && y == 8 && m_northWindEnabled)
                return GetRandomWindVector(randomDir); // North
            else if (x == 7 && y == 7 && m_northwestWindEnabled)
                return GetRandomWindVector(randomDir - Math.PI / 4); // Northwest
            else if (x == 7 && y == 9 && m_northeastWindEnabled)
                return GetRandomWindVector(randomDir + Math.PI / 4); // Northeast
            else if (x == 9 && y == 7 && m_southwestWindEnabled)
                return GetRandomWindVector(randomDir - Math.PI / 4); // Southwest
            else if (x == 9 && y == 9 && m_southeastWindEnabled)
                return GetRandomWindVector(randomDir + Math.PI / 4); // Southeast
            else if (x == 7 && y == 8 && m_westWindEnabled)
                return new Vector2(-1.0f, 0.0f); // West
            else if (x == 8 && y == 7 && m_southWindEnabled)
                return GetRandomWindVector(randomDir + Math.PI); // South
            else if (x == 8 && y == 9 && m_eastWindEnabled)
                return new Vector2(1.0f, 0.0f); // East
            else
                return GetRandomWindVector(randomDir);
        }

        private Vector2 GetRandomWindVector(double baseDirection)
        {
            double randomAngle = baseDirection + (m_angleRange / 180.0 * Math.PI) * (2 * new Random().NextDouble() - 1);
            return new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));
        }

        /// <summary>
        /// Returns the wind speed as a Vector3 at the given coordinates.
        /// </summary>
        /// <param name="fX">The X coordinate.</param>
        /// <param name="fY">The Y coordinate.</param>
        /// <param name="fZ">The Z coordinate.</param>
        /// <returns>Returns the wind speed as a Vector3.</returns>
        public Vector3 WindSpeed(float fX, float fY, float fZ)
        {
            return new Vector3(m_curPredominateWind, 0.0f);
        }

        /// <summary>
        /// Returns an array of Vector2 containing wind speed information.
        /// </summary>
        /// <returns>Returns an array of Vector2 containing wind speed information.</returns>
        public Vector2[] WindLLClientArray()
        {
            return m_windSpeeds;
        }

        /// <summary>
        /// Provides a brief description of the plugin's functionality.
        /// </summary>
        public string Description
        {
            get
            {
                return "Provides a predominate wind direction that can change within configured variances for direction and speed.";
            }
        }

        /// <summary>
        /// Returns a dictionary of wind parameters and their descriptions.
        /// </summary>
        /// <returns>Returns a dictionary of wind parameters and their descriptions.</returns>
        public System.Collections.Generic.Dictionary<string, string> WindParams()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("avgStrength", "average wind strength");
            Params.Add("avgDirection", "average wind direction in degrees");
            Params.Add("varStrength", "allowable variance in wind strength");
            Params.Add("varDirection", "allowable variance in wind direction in +/- degrees");
            Params.Add("rateChange", "rate of change");
            Params.Add("randomWindEnabled", "enable random wind generation (true/false)");
            Params.Add("north_wind_enabled", "enable North wind (true/false)");
            Params.Add("northwest_wind_enabled", "enable Northwest wind (true/false)");
            Params.Add("northeast_wind_enabled", "enable Northeast wind (true/false)");
            Params.Add("south_wind_enabled", "enable South wind (true/false)");
            Params.Add("southwest_wind_enabled", "enable Southwest wind (true/false)");
            Params.Add("southeast_wind_enabled", "enable Southeast wind (true/false)");
            Params.Add("west_wind_enabled", "enable West wind (true/false)");
            Params.Add("east_wind_enabled", "enable East wind (true/false)");
            Params.Add("angle_range", "angle range for random wind direction");
            return Params;
        }

        /// <summary>
        /// Allows you to set specific wind parameters.
        /// </summary>
        /// <param name="param">The name of the parameter to set.</param>
        /// <param name="value">The new value for the parameter.</param>
        public void WindParamSet(string param, float value)
        {
            switch (param)
            {
                case "avgStrength":
                    m_avgStrength = value;
                    break;
                case "avgDirection":
                    m_avgDirection = value;
                    break;
                case "varStrength":
                    m_varStrength = value;
                    break;
                case "varDirection":
                    m_varDirection = value;
                    break;
                case "rateChange":
                    m_rateChange = value;
                    break;
                case "angle_range":
                    m_angleRange = value;
                    break;
                case "north_wind_enabled":
                    m_northWindEnabled = value != 0;
                    break;
                case "northwest_wind_enabled":
                    m_northwestWindEnabled = value != 0;
                    break;
                case "northeast_wind_enabled":
                    m_northeastWindEnabled = value != 0;
                    break;
                case "south_wind_enabled":
                    m_southWindEnabled = value != 0;
                    break;
                case "southwest_wind_enabled":
                    m_southwestWindEnabled = value != 0;
                    break;
                case "southeast_wind_enabled":
                    m_southeastWindEnabled = value != 0;
                    break;
                case "west_wind_enabled":
                    m_westWindEnabled = value != 0;
                    break;
                case "east_wind_enabled":
                    m_eastWindEnabled = value != 0;
                    break;
            }
        }

        /// <summary>
        /// Retrieves the value of a specific wind parameter.
        /// </summary>
        /// <param name="param">The name of the parameter to retrieve.</param>
        /// <returns>Returns the value of the specified wind parameter.</returns>
        public float WindParamGet(string param)
        {
            switch (param)
            {
                case "avgStrength":
                    return m_avgStrength;
                case "avgDirection":
                    return m_avgDirection;
                case "varStrength":
                    return m_varStrength;
                case "varDirection":
                    return m_varDirection;
                case "rateChange":
                    return m_rateChange;
                case "angle_range":
                    return m_angleRange;
                case "north_wind_enabled":
                    return m_northWindEnabled ? 1.0f : 0.0f;
                case "northwest_wind_enabled":
                    return m_northwestWindEnabled ? 1.0f : 0.0f;
                case "northeast_wind_enabled":
                    return m_northeastWindEnabled ? 1.0f : 0.0f;
                case "south_wind_enabled":
                    return m_southWindEnabled ? 1.0f : 0.0f;
                case "southwest_wind_enabled":
                    return m_southwestWindEnabled ? 1.0f : 0.0f;
                case "southeast_wind_enabled":
                    return m_southeastWindEnabled ? 1.0f : 0.0f;
                case "west_wind_enabled":
                    return m_westWindEnabled ? 1.0f : 0.0f;
                case "east_wind_enabled":
                    return m_eastWindEnabled ? 1.0f : 0.0f;
                default:
                    throw new Exception(String.Format("Unknown {0} parameter {1}", this.Name, param));
            }
        }

        /// <summary>
        /// Logs the current wind settings to the log file.
        /// </summary>
        private void LogSettings()
        {
            m_log.InfoFormat("[ConfigurableRandomWind] Average Strength   : {0}", m_avgStrength);
            m_log.InfoFormat("[ConfigurableRandomWind] Average Direction  : {0}", m_avgDirection);
            m_log.InfoFormat("[ConfigurableRandomWind] Variance Strength : {0}", m_varStrength);
            m_log.InfoFormat("[ConfigurableRandomWind] Variance Direction: {0}", m_varDirection);
            m_log.InfoFormat("[ConfigurableRandomWind] Rate Change       : {0}", m_rateChange);
            m_log.InfoFormat("[ConfigurableRandomWind] Random Wind Enabled: {0}", m_randomWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] Wind Angle Range: {0}", m_angleRange);


            m_log.InfoFormat("[ConfigurableRandomWind] north wind enabled: {0}", m_northWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] northwest wind enabled: {0}", m_northwestWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] northeast wind enabled: {0}", m_northeastWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] south wind enabled: {0}", m_southWindEnabled);

            m_log.InfoFormat("[ConfigurableRandomWind] southwest wind enabled: {0}", m_southwestWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] southeast wind enabled: {0}", m_southeastWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] east wind enabled: {0}", m_eastWindEnabled);
            m_log.InfoFormat("[ConfigurableRandomWind] west wind enabled: {0}", m_westWindEnabled);
        }
    }
}
