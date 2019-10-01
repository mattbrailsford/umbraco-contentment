﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public class DataListConfigurationEditor : ConfigurationEditor
    {
        public const string DataSource = "dataSource";
        public const string Items = "items";
        public const string ListEditor = "listEditor";
        internal const string EditorConfig = "editorConfig";

        public DataListConfigurationEditor()
            : base()
        {
            var configEditorViewPath = IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorViewPath);
            var defaultConfigEditorConfig = new Dictionary<string, object>
            {
                { MaxItemsConfigurationField.MaxItems, 1 },
                { DisableSortingConfigurationField.DisableSorting, Constants.Values.True },
                { OverlaySizeConfigurationField.OverlaySize, OverlaySizeConfigurationField.Small },
                { ConfigurationEditorConfigurationEditor.OverlayView, IOHelper.ResolveUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) },
                { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.False },
            };

            var service = new ConfigurationEditorService();
            var dataSources = service.GetConfigurationEditors<IDataListSource>();
            var listEditors = service.GetConfigurationEditors<IDataListEditor>();

            Fields.Add(
                DataSource,
                "Data source",
                "Select and configure the data source.",
                configEditorViewPath,
                new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { ConfigurationEditorConfigurationEditor.Items, dataSources }
                });

            Fields.Add(
                ListEditor,
                "List editor",
                "Select and configure the type of editor for the data list.",
                configEditorViewPath,
                new Dictionary<string, object>(defaultConfigEditorConfig)
                {
                    { ConfigurationEditorConfigurationEditor.Items, listEditors }
                });
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            // NOTE: I am unable to set the `DataListDataValueEditor.View` property from here.
            // The bulk of the configuration code is done in the `DataListDataValueEditor.Configuration` property-setter.
            // Storing the view-editor's config in a temporary dictionary item. Which we then return directly to the view-editor.
            // It feels quite hacky, alas there isn't a viable alternative at present.
            if (config.TryGetValue(EditorConfig, out var tmp) && tmp is Dictionary<string, object> editorConfig)
            {
                return editorConfig;
            }

            return config;
        }
    }
}
