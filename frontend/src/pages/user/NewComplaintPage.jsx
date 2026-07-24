import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { complaintApi } from '../../api/complaintApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { CategoryOptions } from '../../utils/enums';

export default function NewComplaintPage() {
  const [form, setForm] = useState({ title: '', description: '', category: '' });
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const toast = useToast();

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!form.title.trim() || !form.description.trim() || form.category === '') return;

    setLoading(true);
    const res = await complaintApi.create({
      title: form.title.trim(),
      description: form.description.trim(),
      category: Number(form.category),
    });
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Complaint submitted successfully');
      navigate(ROUTES.MY_COMPLAINTS);
    } else {
      toast.error(res.errors?.[0] || 'Failed to submit complaint');
    }
  }

  return (
    <div className="complaint-form-page">
      <Link to={ROUTES.MY_COMPLAINTS} className="back-link">Back to complaints</Link>

      <div className="complaint-form-header">
        <h1>Submit New Complaint</h1>
        <p>Provide enough context for the right team to understand and resolve the issue. Your detailed input helps us expedite the resolution process.</p>
      </div>

      <form className="complaint-form" onSubmit={handleSubmit}>
        <div className="form-card">
          <div className="form-group">
            <div className="form-label-row">
              <label htmlFor="complaint-title">Complaint Title</label>
              <span className="form-counter">{form.title.length}/200 characters</span>
            </div>
            <input
              id="complaint-title"
              type="text"
              name="title"
              placeholder="Briefly summarize the issue"
              value={form.title}
              onChange={handleChange}
              required
              minLength={5}
              maxLength={200}
            />
          </div>

          <div className="form-group">
            <label htmlFor="complaint-category">Category</label>
            <select id="complaint-category" name="category" value={form.category} onChange={handleChange} required>
              <option value="">Select a category</option>
              {CategoryOptions.map((opt) => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <div className="form-label-row">
              <label htmlFor="complaint-description">Full Description</label>
              <span className="form-counter">{form.description.length}/5000 characters</span>
            </div>
            <textarea
              id="complaint-description"
              name="description"
              placeholder="Include dates, locations, affected services, and any steps already taken"
              value={form.description}
              onChange={handleChange}
              required
              minLength={10}
              maxLength={5000}
            />
          </div>
        </div>

        <div className="form-footer">
          <button type="button" className="btn-cancel" onClick={() => navigate(ROUTES.MY_COMPLAINTS)}>
            Cancel
          </button>
          <button type="submit" className="btn-submit" disabled={loading}>
            {loading ? 'Submitting...' : 'Submit Complaint'}
          </button>
        </div>
      </form>

      <aside className="complaint-form-tips">
        <strong>Submission Tips</strong>
        <span>Be as specific as possible</span>
        <span>Attach screenshots if relevant</span>
        <span>Note the exact time of the incident</span>
      </aside>
    </div>
  );
}
