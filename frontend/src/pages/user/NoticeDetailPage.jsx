import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { noticeApi } from '../../api/noticeApi';
import NoticeReadOnlyDetails from '../../components/common/NoticeReadOnlyDetails';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';

export default function NoticeDetailPage() {
  const { id } = useParams();
  const [notice, setNotice] = useState(null);
  const [loading, setLoading] = useState(true);
  const toast = useToast();
  const navigate = useNavigate();

  const loadNotice = useCallback(async () => {
    setLoading(true);
    const result = await noticeApi.getById(id);
    if (result.isSuccess) setNotice(result.data);
    else {
      toast.error(result.errors?.[0] || 'Notice not found');
      navigate(ROUTES.NOTICES);
    }
    setLoading(false);
  }, [id, navigate, toast]);

  useEffect(() => { loadNotice(); }, [loadNotice]);

  if (loading) return <LoadingSpinner />;
  if (!notice) return null;

  return (
    <div className="notice-detail-page">
      <Link to={ROUTES.NOTICES} className="notice-breadcrumb"><span aria-hidden="true">&larr;</span> Notices</Link>
      <article className="detail-card notice-detail-card">
        <span className="eyebrow">Organization Notice</span>
        <h1>Notice Details</h1>
        <NoticeReadOnlyDetails notice={notice} isPublished />
      </article>
    </div>
  );
}
