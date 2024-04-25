import { PlusOutlined } from '@ant-design/icons';
import { FormInstance, ModalForm, ProFormDependency, ProFormSelect, ProFormText } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import ProTable, { ActionType, ProColumns } from '@ant-design/pro-table';
import { Button, message, Modal } from 'antd';
import React, {  useRef, useState } from 'react';
import { getIntl, getLocale } from 'umi';
import { ServiceItem } from './data';
import { addService, queryService, removeService } from './service';
import styles from './index.less';
const { confirm } = Modal;

const handleAdd = async (fields: ServiceItem) => {
  const intl = getIntl(getLocale());
  const hide = message.loading(intl.formatMessage({
    id:'saving'
  }));
  try {
    const result = await addService({ ...fields });
    hide();
    const success = result.success;
    if (success) {
      message.success(intl.formatMessage({
        id:'save_success'
      }));
    } else {
      message.error(result.message);
    }
    return success;
  } catch (error) {
    hide();
    message.error(intl.formatMessage({
      id:'save_fail'
    }));
    return false;
  }
};
const handleDelSome = async (service: ServiceItem):Promise<boolean> => {
  const intl = getIntl(getLocale());
  const hide = message.loading(intl.formatMessage({id:'deleting'}));
  try {
    const result = await removeService(service);
    hide();
    const success = result.success;
    if (success) {
      message.success(intl.formatMessage({id:'delete_success'}));
    } else {
      message.error(intl.formatMessage({id:'delete_fail'}));
    }
    return success;
  } catch (error) {
    hide();
    message.error(intl.formatMessage({id:'delete_fail'}));
    return false;
  }
};

const services: React.FC = () => {
  const actionRef = useRef<ActionType>();
  const addFormRef = useRef<FormInstance>();
  const [createModalVisible, setCreateModalVisible] = useState<boolean>(false);

  const columns: ProColumns[] = [
    {
      title: 'Service ID',
      dataIndex: 'serviceId',
    },
    {
      title: 'Service Name',
      dataIndex: 'serviceName',
      sorter: true,
    },
    {
      title: 'IP',
      dataIndex: 'ip',
      hideInSearch: true,
    },
    {
      title: 'Port',
      dataIndex: 'port',
      hideInSearch: true,
    },
    {
      title: 'Metadata',
      dataIndex: 'metaData',
      hideInSearch: true,
    },
    {
      title: 'HC Mode',
      dataIndex: 'heartBeatMode',
      hideInSearch: true,
    },
    {
      title: 'CheckUrl',
      dataIndex: 'checkUrl',
      hideInSearch: true,
      ellipsis: true,
    },
    {
      title: 'AlarmUrl',
      dataIndex: 'alarmUrl',
      hideInSearch: true,
      ellipsis: true,
    },
    {
      title: 'RegisterTime',
      dataIndex: 'registerTime',
      hideInSearch: true,
      valueType: 'dateTime',
      sorter: true,
    },
    {
      title: 'Last HC',
      dataIndex: 'lastHeartBeat',
      hideInSearch: true,
      valueType: 'dateTime',
    },
    {
      title: 'Status',
      dataIndex: 'status',
      valueEnum: {
        0: {
          text: 'Default',
          status: 'Default'
        },
        1: {
          text: 'Success',
          status: 'Success'
        }
      },
      width: 120
    },
    {
      title: 'operating',
      valueType: 'option',
      render: (text, record, _, action) => [
        <a className={styles.linkDanger}
          onClick={
            ()=>{
              confirm({
                content:`Are you sure you want to delete the selected service?`,
                onOk: async ()=>{
                  const result = await handleDelSome(record)
                  if (result) {
                    actionRef.current?.reload();
                  }
                }
              })
            }
          }
        >
          Delete
        </a>
      ]
    }
  ];
  return (
    <PageContainer>
      <ProTable
        search={{
          labelWidth: 'auto',
        }}
        actionRef={actionRef}
        options={
          false
        }
        rowKey="id"
        columns={columns}
        request={(params, sorter, filter) => {
          let sortField = 'registerTime';
          let ascOrDesc = 'descend';
          for (const key in sorter) {
            sortField = key;
            const val = sorter[key];
            if (val) {
              ascOrDesc = val;
            }
          }
          console.log(sortField, ascOrDesc);
          return queryService({ sortField, ascOrDesc, ...params })
        }}
        toolBarRender={()=>
          [
            <Button key="button" icon={<PlusOutlined />} type="primary" onClick={() => { setCreateModalVisible(true) }}>
              New Service
            </Button>
          ]
        }
      />
      <ModalForm
        formRef={addFormRef}
        title='New Service'
        visible={createModalVisible}
        onVisibleChange={setCreateModalVisible}
        onFinish={
          async (value) => {
            const success = await handleAdd(value as ServiceItem);
            if (success) {
              setCreateModalVisible(false);
              if (actionRef.current) {
                actionRef.current.reload();
              }
            }
            addFormRef.current?.resetFields();
          }
        }
      >
        <ProFormText
          rules={[
            {
              required: true,
            },
          ]}
          label='Service Id'
          name="serviceId"
        />
        <ProFormText
          rules={[
            {
              required: true,
            },
          ]}
          label='Service Name'
          name="serviceName"
        />
        <ProFormSelect
                rules={[
                  {
                    required: true,
                  },
                ]}
                 tooltip={
                   ()=>{
                     return <div>
                       none: This mode does not perform any health checks and the service will always be online<br/>
                       client: client proactively reports<br/>
                       server: server side active detection
                     </div>
                   }
                 }
                  label="Health check mode"
                  name="heartBeatMode"
                  request={ async () => {
                    return [
                      {
                        label:'none',
                        value: 'none',
                      },
                      {
                        label:'client',
                        value: 'client'
                      },
                      {
                        label:'server',
                        value: 'server'
                      }
                    ];
                  }}
        >
        </ProFormSelect>
        <ProFormText
          rules={[
            {
            },
          ]}
          label='IP'
          name="ip"
        />
        <ProFormText
          rules={[
            {
            },
          ]}
          label='Port'
          name="port"
        />
        <ProFormDependency
          name={
            ["heartBeatMode"]
          }
        >
          {
            (e)=>{
              return e.heartBeatMode == 'server'? <ProFormText
                  rules={[
                    {
                      required: true,
                    },
                  ]}
                  label='Check URL'
                  name="checkUrl"
                />: null
            }
          }
        </ProFormDependency>
        <ProFormDependency
          name={
            ["heartBeatMode"]
          }
        >
          {
            (e)=>{
              return (e.heartBeatMode == 'none' || e.heartBeatMode == null)? null:<ProFormText
                  rules={[
                    {
                    },
                  ]}
                  label='Alarm URL'
                  name="alarmUrl"
                />
            }
          }
        </ProFormDependency>
      </ModalForm>
    </PageContainer>
  );
}
export default services;
