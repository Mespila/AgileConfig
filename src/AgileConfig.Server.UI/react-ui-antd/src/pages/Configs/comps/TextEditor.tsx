import { Button, message, Modal, Input, Checkbox, Tooltip } from 'antd';
const { TextArea } = Input;
import Editor from '@monaco-editor/react';
import React, { useState, useEffect } from 'react';
import { getConfigsKvList, saveKvList } from '../service';
import { CheckboxChangeEvent } from 'antd/es/checkbox';
import { QuestionCircleOutlined } from '@ant-design/icons';
export type TextEditorProps = {
  appId: string;
  appName: string;
  ModalVisible: boolean;
  env: string;
  onCancel: () => void;
  onSaveSuccess: () => void;
};
const handleSave = async (
  kvText: string | undefined,
  appId: string,
  env: string,
  isPatch: boolean,
) => {
  const hide = message.loading('正在保存...');
  try {
    const result = await saveKvList(appId, env, kvText ? kvText : '', isPatch);
    hide();
    const success = result.success;
    if (success) {
      message.success('Saved successfully!');
    } else {
      message.error(result.message ? result.message : 'Save failed!');
    }
    return success;
  } catch (error) {
    hide();
    message.error('Save failed!');
    return false;
  }
};
const TextEditor: React.FC<TextEditorProps> = (props) => {
  const [kvText, setkvText] = useState<string | undefined>('');
  const [isPatch, setIsPatch] = useState<boolean>(false);

  const onIsPatchChange = (e: CheckboxChangeEvent) => {
    setIsPatch(e.target.checked);
  };
  useEffect(() => {
    getConfigsKvList(props.appId, props.env).then((res) => {
      if (res.success) {
        const list = res.data.map((x: { value: string; key: string }) => x.key + '=' + x.value);
        setkvText(list.join('\n'));
      }
    });
  }, []);

  return (
    <Modal
      maskClosable={false}
      title="Edit by TEXT view"
      okText="保存"
      width={800}
      visible={props.ModalVisible}
      onCancel={() => {
        props.onCancel();
      }}
      footer={
        <div style={{ display: 'flex', justifyContent: 'space-between' }}>
          <div
            style={{
              color: '#999',
              fontSize: '12px',
            }}
          >
            Edit strictly according to the KEY=VALUE format, one configuration per line
          </div>
          <div>
            <span style={{ marginRight: '12px' }}>
              <Checkbox onChange={onIsPatchChange} value={isPatch}>
                Patch Mode
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
                const saveResult = await handleSave(kvText, props.appId, props.env, isPatch);
                if (saveResult) {
                  props.onSaveSuccess();
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
        defaultLanguage="text"
        defaultValue=""
        value={kvText}
        options={{ minimap: { enabled: false } }}
        onChange={(v, e) => {
          setkvText(v);
        }}
      />
    </Modal>
  );
};

export default TextEditor;
