import { Button, message, Modal, Input, Checkbox, Tooltip } from 'antd';
const { TextArea } = Input;
import React, { useState, useEffect } from 'react';
import { getConfigJson, saveJson } from '../service';
import Editor from '@monaco-editor/react';
import { loader } from '@monaco-editor/react';
import { CheckboxChangeEvent } from 'antd/lib/checkbox/Checkbox';
import { QuestionCircleOutlined } from '@ant-design/icons';
loader.config({ paths: { vs: 'monaco-editor/min/vs' } });

const handleSave = async (json: string, appId: string, env: string, isPatch: boolean) => {
  const hide = message.loading('Saving...');
  try {
    const result = await saveJson(appId, env, json, isPatch);
    hide();
    const success = result.success;
    if (success) {
      message.success('Saved successfully！');
    } else {
      message.error('Save failed！');
    }
    return success;
  } catch (error) {
    hide();
    message.error('Save failed！');
    return false;
  }
};

export type JsonEditorProps = {
  appId: string;
  appName: string;
  ModalVisible: boolean;
  env: string;
  onCancel: () => void;
  onSaveSuccess: () => void;
};

const JsonEditor: React.FC<JsonEditorProps> = (props) => {
  const [json, setJson] = useState<string>();
  const [jsonValidateSuccess, setJsonValidateSuccess] = useState<boolean>(true);
  const [isPatch, setIsPatch] = useState<boolean>(false);

  const onIsPatchChange = (e: CheckboxChangeEvent) => {
    setIsPatch(e.target.checked);
  };

  useEffect(() => {
    getConfigJson(props.appId, props.env).then((res) => {
      if (res.success) {
        let jsonObj = JSON.parse(res.data);
        console.log(jsonObj);
        setJson(res.data);
        console.log(res.data);
        console.log(json);
      }
    });
  }, []);
  return (
    <Modal
      maskClosable={false}
      title="Edit by JSON view"
      okText="Save"
      width={800}
      visible={props.ModalVisible}
      onCancel={props.onCancel}
      footer={
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <div>
            {jsonValidateSuccess ? <></> : <span style={{ color: 'red' }}>JSON format is illegal</span>}
          </div>
          <div>
            <span style={{ marginRight: '12px' }}>
              <Checkbox onChange={onIsPatchChange} value={isPatch}>
                Patch mode update
              </Checkbox>
              <Tooltip title="In patch mode, only configurations will be added or modified. Existing configuration items not included above will not be deleted.">
                <QuestionCircleOutlined />
              </Tooltip>
            </span>

            <Button
              onClick={() => {
                props.onCancel();
              }}
            >
              Cancel
            </Button>
            <Button
              type="primary"
              onClick={async () => {
                if (json && jsonValidateSuccess) {
                  const saveResult = await handleSave(json, props.appId, props.env, isPatch);
                  if (saveResult) {
                    props.onSaveSuccess();
                  }
                } else {
                  message.error('JSON format is illegal');
                }
              }}
            >
              Save
            </Button>
          </div>
        </div>
      }
    >
      <Editor
        height="500px"
        defaultLanguage="json"
        defaultValue=""
        value={json}
        options={{ minimap: { enabled: false } }}
        beforeMount={(monaco) => {
          monaco.languages.json.jsonDefaults.setDiagnosticsOptions({
            validate: true,
            allowComments: true,//是否允许json内容中带注释
            schemaValidation: 'error',
          });
        }}
        onChange={(v, e) => {
          setJson(v);
        }}
        onValidate={(markers) => {
          setJsonValidateSuccess(markers.length == 0);
        }}
      />
    </Modal>
  );
};

export default JsonEditor;
