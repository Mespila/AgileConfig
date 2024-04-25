import { getEnvList } from "@/utils/system";
import { Button, message, Modal, Space, Table, Upload, Checkbox } from "antd";
import { CheckboxValueType } from "antd/lib/checkbox/Group";
import React, { useState } from 'react';
import { envSync } from "../service";
const CheckboxGroup = Checkbox.Group;
export type EnvSyncFormProps = {
    appId: string,
    currentEnv: string,
    ModalVisible: boolean;
    onCancel: () => void;
    onSaveSuccess: ()=> void;
  };

const EnvSync : React.FC<EnvSyncFormProps> = (props)=>{
    const [checkedList, setCheckedList] = React.useState<CheckboxValueType[]>([]);
    const envList = getEnvList();
    const onChange = (list:CheckboxValueType[]) => {
      setCheckedList(list);
    };

    return (
        <Modal
          title="同步环境"
          visible={props.ModalVisible}
          onCancel={
            ()=>{
              props.onCancel();
            }
          }
          onOk={
            async ()=> {
              if (!checkedList.length) {
                message.error('Please check at least one environment');
                return;
              }
              const hide = message.loading('Synchronizing');
              try {
                const result = await envSync(props.appId, props.currentEnv, checkedList.map(item=>item.toString()));
                const success = result.success;
                if (success) {
                  props.onSaveSuccess();
                  message.success('Synchronization successful！');
                } else {
                  message.error('Sync failed');
                }
              }
              catch (e) {
                message.error('Sync failed');
              }
              finally {
                hide();
              }
            }
          }
          >
          Synchronize the configuration of the current {props.currentEnv} environment to：
          <div style={{marginTop:20}}>
            <CheckboxGroup options={envList.filter(x=> x !== props.currentEnv)} value={checkedList} onChange={onChange}  />
          </div>
        </Modal>
    );
}

export default EnvSync;
