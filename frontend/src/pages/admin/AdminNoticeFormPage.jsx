import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { adminNoticeApi } from '../../api/noticeApi';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';

const initialForm = { title: '', priority: 'Normal', content: '' };

export default function AdminNoticeFormPage() {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const [form, setForm] = useState(initialForm);
  const [isPublished, setIsPublished] = useState(false);
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const navigate = useNavigate();
  const toast = useToast();

  const loadNotice = useCallback(async () => {
    setLoading(true);
    const result = await adminNoticeApi.getById(id);
    if (result.isSuccess) {
      setForm({
        title: result.data.title || '',
        priority: result.data.priority || 'Normal',
        content: result.data.content || '',
      });
      setIsPublished(Boolean(result.data.isPublished));
    } else {
      toast.error(result.errors?.[0] || 'Notice not found');
      navigate(ROUTES.ADMIN_NOTICES);
    }
    setLoading(false);
  }, [id, navigate, toast]);

  useEffect(() => {
    if (isEdit) loadNotice();
  }, [isEdit, loadNotice]);

  function updateField(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    const action = event.nativeEvent.submitter?.value || 'save';

    if (!form.title.trim() || !form.content.trim()) {
      toast.error('A title and notice content are required.');
      return;
    }

    setSaving(true);
    const payload = { title: form.title.trim(), content: form.content.trim(), priority: Number(['Normal', 'Important', 'Urgent'].indexOf(form.priority)) };

    let result;
    if (isEdit) {
      result = await adminNoticeApi.update(id, payload);
      if (result.isSuccess && action === 'publish') result = await adminNoticeApi.publish(id);
      if (result.isSuccess && action === 'unpublish') result = await adminNoticeApi.unpublish(id);
    } else {
      result = await adminNoticeApi.create(payload);
      if (result.isSuccess && action === 'publish') result = await adminNoticeApi.publish(result.data.id);
    }

    if (result.isSuccess) {
      const message = action === 'publish'
        ? 'Notice published successfully.'
        : action === 'draft'
          ? 'Draft saved successfully.'
          : action === 'unpublish'
            ? 'Notice unpublished successfully.'
            : isEdit
              ? 'Notice updated successfully.'
              : 'Notice created successfully.';
      toast.success(message);
      navigate(ROUTES.ADMIN_NOTICES);
    } else {
      toast.error(result.errors?.[0] || 'Unable to save notice');
    }
    setSaving(false);
  }

  if (loading) return <LoadingSpinner />;

  const currentStatusAction = isPublished ? 'unpublish' : 'publish';

  return (
    <div className="notice-form-page">
      <Link to={isEdit ? ROUTES.ADMIN_NOTICE_DETAIL.replace(':id', id) : ROUTES.ADMIN_NOTICES} className="back-link">Back to notices</Link>
      <div className="page-header">
        <h1>{isEdit ? 'Edit Notice' : 'Create Notice'}</h1>
        <p>{isEdit ? 'Update the content or publication status of this organization notice.' : 'Prepare a draft or publish an update for all organization members.'}</p>
      </div>

      <form className="notice-form" onSubmit={handleSubmit}>
        <div className="form-card">
          <div className="form-group">
            <label htmlFor="notice-title">Title</label>
            <input id="notice-title" name="title" maxLength="200" value={form.title} onChange={updateField} placeholder="Enter a clear notice title" />
          </div>
          <div className="form-group">
            <label htmlFor="notice-priority">Priority</label>
            <select id="notice-priority" name="priority" value={form.priority} onChange={updateField}>
              <option value="Normal">Normal</option>
              <option value="Important">Important</option>
              <option value="Urgent">Urgent</option>
            </select>
          </div>
          <div className="form-group">
            <div className="form-label-row"><label htmlFor="notice-content">Content</label><span className="form-counter">{form.content.length}/10,000</span></div>
            <textarea id="notice-content" name="content" maxLength="10000" value={form.content} onChange={updateField} placeholder="Write the full notice for members of your organization." />
          </div>
        </div>
        <div className="form-footer">
          <button type="button" className="btn-cancel" onClick={() => navigate(ROUTES.ADMIN_NOTICES)}>Cancel</button>
          {isEdit ? (
            <>
              <button type="submit" className="btn-outline" value="save" disabled={saving}>Update</button>
              <button type="submit" className="btn-submit" value={currentStatusAction} disabled={saving}>
                {currentStatusAction === 'publish' ? 'Publish' : 'Unpublish'}
              </button>
            </>
          ) : (
            <>
              <button type="submit" className="btn-outline" value="draft" disabled={saving}>Save Draft</button>
              <button type="submit" className="btn-submit" value="publish" disabled={saving}>Publish</button>
            </>
          )}
        </div>
      </form>
    </div>
  );
}
